namespace Apiand.TemplateEngine.Models;

public class ValidationResult
{
    public bool IsValid => Errors.Count == 0;
    public List<string> Errors { get; } = new();

    public void AddError(string error)
    {
        Errors.Add(error);
    }

    public void AddErrors(params string?[] error)
    {
        foreach (var err in error.Where(e => e != null))
            Errors.Add(err!);
    }
}