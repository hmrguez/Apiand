namespace Apiand.TemplateEngine.Models;

public interface ICommandSpecification
{
    public string ArchName { get; set; }
    void Handle(string s, string workingDirectory, string argument, Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger);
}