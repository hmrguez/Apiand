using System.Diagnostics;
using System.Text.Json;

namespace Apiand.TemplateEngine;

public class TemplateProcessor
{
    private readonly TemplateEngine _engine = new();
    
    public void CreateFromTemplateVariants(Dictionary<string, string> templatePaths, string outputPath, Dictionary<string, object> data, TemplateConfiguration config)
    {
        // Create base output directory
        Directory.CreateDirectory(outputPath);
    
        foreach (var (module, templatePath) in templatePaths)
        {
            // Read template metadata
            var metadata = ReadTemplateMetadata(templatePath);
        
            // Determine module output directory (might be same as base or a subdirectory)
            string moduleOutputPath = config.Architecture == "single-layer" 
                ? outputPath 
                : Path.Combine(outputPath, module);
            
            // Create module directory
            Directory.CreateDirectory(moduleOutputPath);
            
            // Process this module template
            CopyAndProcessDirectory(templatePath, moduleOutputPath, data, metadata);
        
            // Execute post-actions if any
            if (metadata.PostActions?.Count > 0)
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
            return System.Text.Json.JsonSerializer.Deserialize<TemplateMetadata>(json) ?? new TemplateMetadata();
        }
    
        return new TemplateMetadata();
    }
                
    public void CreateFromTemplate(string templatePath, string outputDir, object templateData, TemplateConfiguration configuration)
    {
        // Ensure output directory exists
        Directory.CreateDirectory(outputDir);
                    
        // Process template metadata if exists
        var metadataPath = Path.Combine(templatePath, "template.json");
        TemplateMetadata metadata = File.Exists(metadataPath) 
            ? JsonSerializer.Deserialize<TemplateMetadata>(File.ReadAllText(metadataPath))! 
            : new TemplateMetadata();
                    
        // Copy and process all files recursively
        CopyAndProcessDirectory(templatePath, outputDir, templateData, metadata);
                    
        // Run post-creation commands if any
        if (metadata.PostActions?.Count > 0)
        {
            ExecutePostActions(outputDir, metadata.PostActions);
        }
    }
                
    private void CopyAndProcessDirectory(string sourcePath, string targetPath, object data, TemplateMetadata metadata)
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
                
    private string ProcessName(string name, object data)
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
            
public class TemplateMetadata
{
    public string Name { get; set; } = "Unnamed Template";
    public string Description { get; set; } = "";
    public List<string> TextFileExtensions { get; set; } = [".cs", ".json", ".xml", ".csproj", ".txt", ".md"];
    public List<string> ExcludeFiles { get; init; } = [];
    public List<string> ExcludeDirs { get; init; } = [];
    public List<string> PostActions { get; init; } = [];
}