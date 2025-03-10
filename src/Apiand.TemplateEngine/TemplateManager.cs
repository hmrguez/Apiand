using System.Text.Json;

namespace Apiand.TemplateEngine;

public class TemplateManager
{
    private readonly string _templatesDirectory;
    
    public TemplateManager(string templatesDirectory)
    {
        _templatesDirectory = templatesDirectory;
        Directory.CreateDirectory(_templatesDirectory);
    }
    
    public IEnumerable<TemplateInfo> ListTemplates()
    {
        foreach (var dir in Directory.GetDirectories(_templatesDirectory))
        {
            var metadataPath = Path.Combine(dir, "template.json");
            if (File.Exists(metadataPath))
            {
                var metadata = JsonSerializer.Deserialize<TemplateMetadata>(File.ReadAllText(metadataPath));
                yield return new TemplateInfo
                {
                    Id = Path.GetFileName(dir),
                    Name = metadata.Name,
                    Description = metadata.Description,
                    Path = dir
                };
            }
        }
    }
    
    public string GetTemplatePath(string templateId)
    {
        string path = Path.Combine(_templatesDirectory, templateId);
        return Directory.Exists(path) ? path : null;
    }
    
    public bool InstallTemplate(string sourcePath, string templateId)
    {
        if (!Directory.Exists(sourcePath))
            return false;
            
        string targetPath = Path.Combine(_templatesDirectory, templateId);
        if (Directory.Exists(targetPath))
            Directory.Delete(targetPath, true);
            
        DirectoryCopy(sourcePath, targetPath, true);
        return true;
    }
    
    private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
    {
        // Standard recursive directory copy implementation
        DirectoryInfo dir = new DirectoryInfo(sourceDirName);
        DirectoryInfo[] dirs = dir.GetDirectories();

        if (!dir.Exists)
            throw new DirectoryNotFoundException($"Source directory does not exist or could not be found: {sourceDirName}");

        if (!Directory.Exists(destDirName))
            Directory.CreateDirectory(destDirName);

        FileInfo[] files = dir.GetFiles();
        foreach (FileInfo file in files)
        {
            string temppath = Path.Combine(destDirName, file.Name);
            file.CopyTo(temppath, true);
        }

        if (copySubDirs)
        {
            foreach (DirectoryInfo subdir in dirs)
            {
                string temppath = Path.Combine(destDirName, subdir.Name);
                DirectoryCopy(subdir.FullName, temppath, true);
            }
        }
    }
}

public class TemplateInfo
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Path { get; set; }
}