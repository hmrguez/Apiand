namespace Apiand.TemplateEngine.Models;

public interface IMessenger
{
    void WriteStatusMessage(string message);
    void WriteSuccessMessage(string message);
    void WriteErrorMessage(string message);
}