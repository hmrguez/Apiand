namespace Apiand.TemplateEngine.Ai;

public interface IAiService
{
    /// <summary>
    /// Sends a prompt to the AI service and returns the response
    /// </summary>
    /// <param name="prompt">The user prompt to send</param>
    /// <param name="systemPrompt">Optional system prompt to set context</param>
    /// <returns>The AI response text</returns>
    Task<string> PromptAsync(string prompt, string systemPrompt = "You are a helpful assistant.");
    
    /// <summary>
    /// Fills a code template with AI-generated content based on the prompt
    /// </summary>
    /// <param name="codeTemplate">The code template with placeholders to fill</param>
    /// <param name="prompt">The prompt describing what to generate</param>
    /// <param name="systemPrompt">Optional system prompt to set context</param>
    /// <returns>The filled code template</returns>
    Task<string> FillTemplateAsync(string codeTemplate, string prompt, string systemPrompt = "You are a helpful coding assistant.");
    
    /// <summary>
    /// Fills multiple code templates with AI-generated content based on a unified prompt
    /// </summary>
    /// <param name="templates">Dictionary of template names to code templates with placeholders</param>
    /// <param name="prompt">The unified prompt describing what to generate for all templates</param>
    /// <param name="systemPrompt">Optional system prompt to set context</param>
    /// <returns>Dictionary with template names and their filled content</returns>
    Task<Dictionary<string, string>> FillMultipleTemplatesAsync(
        Dictionary<string, string> templates, 
        string prompt,
        string systemPrompt = "You are a helpful coding assistant.");
}
