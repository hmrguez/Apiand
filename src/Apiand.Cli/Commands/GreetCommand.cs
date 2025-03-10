using System.CommandLine;

namespace Apiand.Cli.Commands;

public class GreetCommand : Command
{
    public GreetCommand() : base("greet", "Greets the user")
    {
        var nameOption = new Option<string>("--name", "The name of the person to greet");
        nameOption.AddAlias("-n");
        AddOption(nameOption);

        this.SetHandler(HandleCommand, nameOption);
    }

    private void HandleCommand(string name)
    {
        // Create an instance of the template engine
        var engine = new TemplateEngine.TemplateEngine();
        
        // Define a template with XXX markers
        string template = "Hello, XXXnameXXX! Welcome to XXXcompanyXXX.";
        
        // Create a data object
        var data = new { name = "John", company = "Apiand" };
        
        // Render the template
        string result = engine.Render(template, data);
        Console.WriteLine(result);
        
        
        // result will be: "Hello, John! Welcome to Apiand."
        
        // Console.WriteLine($"Hello {name}!");
    }
}