using Scriban;
using System.IO;
using System.Threading.Tasks;

public class TemplateService
{
    private readonly string _templateFolder;
    private readonly string _outputFolder;

    public TemplateService(string templateFolder, string outputFolder)
    {
        _templateFolder = templateFolder;
        _outputFolder = outputFolder;
    }

    public async Task ProcessTemplatesAsync(object model)
    {
        var templateFiles = Directory.GetFiles(_templateFolder, "*.scriban");

        foreach (var templateFile in templateFiles)
        {
            var templateContent = await File.ReadAllTextAsync(templateFile);
            var template = Template.Parse(templateContent);

            var result = template.Render(model);

            var outputFileName = Path.GetFileNameWithoutExtension(templateFile) + ".txt";
            var outputFilePath = Path.Combine(_outputFolder, outputFileName);

            await File.WriteAllTextAsync(outputFilePath, result);
        }
    }
}