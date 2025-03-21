using System.Diagnostics;
using System.Text.Json;
using Apiand.TemplateEngine.Models;

namespace Apiand.TemplateEngine;

public class TemplateProcessor
{
    private readonly TemplateEngine _engine = new();
    
    public void CreateFromTemplateVariants(Dictionary<string, string> templatePaths, string outputPath, Dictionary<string, string> data)
    {
        Directory.CreateDirectory(outputPath);
    
        foreach (var (module, templatePath) in templatePaths)
        {
            var metadata = ReadTemplateMetadata(templatePath);
        
            string moduleOutputPath = Path.Combine(outputPath, module);
            
            Directory.CreateDirectory(moduleOutputPath);
            
            CopyAndProcessDirectory(templatePath, moduleOutputPath, data, metadata);
        
            if (metadata.PostActions.Count > 0)
            {
                ExecutePostActions(moduleOutputPath, metadata.PostActions);
            }
        }
    }

    private TemplateMetadata ReadTemplateMetadata(string templatePath)
    {
        string metadataPath = Path.Combine(templatePath, "template.json");
        if (File.Exists(metadataPath))
        {
            string json = File.ReadAllText(metadataPath);
            return JsonSerializer.Deserialize<TemplateMetadata>(json) ?? new TemplateMetadata();
        }
    
        return new TemplateMetadata();
    }
                
    private void CopyAndProcessDirectory(string sourcePath, string targetPath, Dictionary<string, string> data, TemplateMetadata metadata)
    {
        // Get source directories
        foreach (var directory in Directory.GetDirectories(sourcePath))
        {
            // Skip special directories
            string dirName = Path.GetFileName(directory);
            if (dirName == ".template" || metadata.ExcludeDirs?.Contains(dirName) == true)
                continue;
                            
            // Process directory name (might contain variables)
            string processedDirName = ProcessName(dirName, data);
            string newDirPath = Path.Combine(targetPath, processedDirName);
                        
            // Create directory
            Directory.CreateDirectory(newDirPath);
                        
            // Recursively process subdirectories
            CopyAndProcessDirectory(directory, newDirPath, data, metadata);
        }
                    
        // Get source files
        foreach (var file in Directory.GetFiles(sourcePath))
        {
            string fileName = Path.GetFileName(file);
                        
            // Skip metadata and excluded files
            if (fileName == "template.json" || metadata.ExcludeFiles?.Contains(fileName) == true)
                continue;
                            
            // Process filename (might contain variables)
            string processedFileName = ProcessName(fileName, data);
            string targetFilePath = Path.Combine(targetPath, processedFileName);
                        
            // Process content and write to target
            if (metadata.TextFileExtensions?.Any(ext => fileName.EndsWith(ext, StringComparison.OrdinalIgnoreCase)) != false)
            {
                // Process as template
                string content = File.ReadAllText(file);
                string processedContent = _engine.Render(content, data);
                File.WriteAllText(targetFilePath, processedContent);
            }
            else
            {
                // Binary file, just copy
                File.Copy(file, targetFilePath, true);
            }
        }
    }
                
    private string ProcessName(string name, Dictionary<string, string> data)
    {
        // Process names that contain variables like "XXXprojectNameXXX.csproj"
        return _engine.Render(name, data);
    }
                
    private void ExecutePostActions(string workingDir, List<string> commands)
    {
        foreach (var command in commands)
        {
            var parts = command.Split(' ', 2);
            var psi = new ProcessStartInfo
            {
                FileName = parts[0],
                Arguments = parts.Length > 1 ? parts[1] : "",
                WorkingDirectory = workingDir,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            };
                        
            using var process = Process.Start(psi);
            process?.WaitForExit();
        }
    }
}