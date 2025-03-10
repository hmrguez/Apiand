using System.CommandLine;
using Apiand.TemplateEngine;

namespace Apiand.Cli.Commands;

public class NewCommand : Command
{
    private readonly TemplateManager _templateManager;
    private readonly TemplateProcessor _processor;
    
    public NewCommand() : base("new", "Creates a new project from a template")
    {
        _templateManager = new TemplateManager();
        _processor = new TemplateProcessor();
        
        var templateOption = new Option<string>("--template", "The template to use") { IsRequired = true };
        templateOption.AddAlias("-t");
        
        var outputOption = new Option<string>("--output", "The output directory") { IsRequired = true };
        outputOption.AddAlias("-o");
        
        var nameOption = new Option<string>("--name", "Project name");
        nameOption.AddAlias("-n");
        
        AddOption(templateOption);
        AddOption(outputOption);
        AddOption(nameOption);
        
        this.SetHandler(HandleCommand, templateOption, outputOption, nameOption);
    }
    
    private void HandleCommand(string templateId, string output, string name)
    {
        var templatePath = _templateManager.GetTemplatePath(templateId);
        if (templatePath == null)
        {
            Console.WriteLine($"Template '{templateId}' not found.");
            return;
        }
        
        var data = new Dictionary<string, object>
        {
            ["name"] = name ?? Path.GetFileName(Path.GetFullPath(output)),
            // Add more default data as needed
        };
        
        _processor.CreateFromTemplate(templatePath, output, data);
        Console.WriteLine($"Project created in {output}");
    }
}