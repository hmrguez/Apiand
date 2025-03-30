using Apiand.Extensions.Models;

namespace Apiand.TemplateEngine.Models;

public interface ICommandSpecification
{
    public string ArchName { get; set; }
    Result Handle(string workingDirectory, string projectDirectory, string argument,
        Dictionary<string, string> extraData,
        TemplateConfiguration configuration, IMessenger messenger);
}