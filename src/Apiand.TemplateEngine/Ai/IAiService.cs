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
}
