using System.CommandLine;

// Create root command
var rootCommand = new RootCommand("Apiand CLI tool");

// Add a subcommand
var greetCommand = new Command("greet", "Greets the user");
var nameOption = new Option<string>("--name", "The name of the person to greet");
nameOption.AddAlias("-n");
greetCommand.AddOption(nameOption);

greetCommand.SetHandler((string name) =>
{
    Console.WriteLine($"Hello {name}!");
}, nameOption);

// Add another subcommand example
var versionCommand = new Command("version", "Displays the version of the tool");
versionCommand.SetHandler(() =>
{
    Console.WriteLine("Apiand CLI v1.0.0");
});

// Add commands to root
rootCommand.AddCommand(greetCommand);
rootCommand.AddCommand(versionCommand);

// Run the command
return await rootCommand.InvokeAsync(args);