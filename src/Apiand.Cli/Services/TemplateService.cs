using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reflection;

namespace Apiand.Cli.Services;

public class TemplateService
{
    private readonly string _templateRootPath;

    public TemplateService(string? customTemplatePath = null)
    {

        if (!string.IsNullOrEmpty(customTemplatePath) && Directory.Exists(customTemplatePath))
        {
            _templateRootPath = customTemplatePath;
        }
        else
        {
            string executablePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? 
                                    AppContext.BaseDirectory;
            string embeddedTemplatesPath = Path.Combine(executablePath, "Templates");

            if (Directory.Exists(embeddedTemplatesPath))
            {
                _templateRootPath = embeddedTemplatesPath;
            }
            else
            {
                string currentDirTemplatesPath = Path.Combine(Directory.GetCurrentDirectory(), "Templates");
                if (Directory.Exists(currentDirTemplatesPath))
                {
                    _templateRootPath = currentDirTemplatesPath;
                }
                else
                {
                    throw new DirectoryNotFoundException("Templates directory not found in any of the expected locations.");
                }
            }
        }
    }

    public async Task ProcessTemplatesAsync(string outputDirectory, Dictionary<string, string> replacements)
    {
        if (string.IsNullOrEmpty(outputDirectory))
        {
            throw new ArgumentException("Output directory cannot be null or empty", nameof(outputDirectory));
        }

        // Ensure output directory exists
        Directory.CreateDirectory(outputDirectory);

        // Get all template files with their relative paths
        var templateFiles = GetAllTemplateFiles(_templateRootPath);

        foreach (var templateFile in templateFiles)
        {
            await ProcessTemplateFileAsync(templateFile, outputDirectory, replacements);
        }
    }

    private IEnumerable<string> GetAllTemplateFiles(string templateDir)
    {
        return Directory.GetFiles(templateDir, "*.*", SearchOption.AllDirectories);
    }

    private async Task ProcessTemplateFileAsync(string templatePath, string outputDirectory, Dictionary<string, string> replacements)
    {
        // Calculate relative path to preserve directory structure
        string relativePath = Path.GetRelativePath(_templateRootPath, templatePath);
        string targetFilePath = Path.Combine(outputDirectory, relativePath);

        // Ensure target directory exists
        string targetDirectory = Path.GetDirectoryName(targetFilePath)!;
        if (!string.IsNullOrEmpty(targetDirectory))
        {
            Directory.CreateDirectory(targetDirectory);
        }

        // Read template content
        string content = await File.ReadAllTextAsync(templatePath);

        // Replace placeholders
        foreach (var replacement in replacements)
        {
            content = content.Replace($"{{{{{replacement.Key}}}}}", replacement.Value);
        }

        // Write processed content to target file
        await File.WriteAllTextAsync(targetFilePath, content);
    }

    public IEnumerable<string> GetTemplateDirectories()
    {
        return Directory.GetDirectories(_templateRootPath, "*", SearchOption.TopDirectoryOnly);
    }
}