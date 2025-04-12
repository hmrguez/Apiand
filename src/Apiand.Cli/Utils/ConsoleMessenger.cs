using Apiand.TemplateEngine.Models;

namespace Apiand.Cli.Utils;

public class ConsoleMessenger : IMessenger
{
    public void WriteStatusMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.Write("→ ");
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteSuccessMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("\u2728 ");
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteErrorMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("✗ ");
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public void WriteWarningMessage(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write("⚠ ");
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}