using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Apiand.TemplateEngine.Ai;

public class AiService : IAiService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;
    private readonly string _model;

    public AiService(string endpoint = "http://localhost:12434/engines/llama.cpp/v1/chat/completions",
        string model = "ai/smollm2")
    {
        _httpClient = new HttpClient();
        _endpoint = endpoint;
        _model = model;
    }

    public async Task<string> PromptAsync(string prompt, string systemPrompt = "You are a helpful assistant.")
    {
        var chatRequest = new ChatRequest
        {
            Model = _model,
            Messages = new List<Message>
            {
                new Message { Role = "system", Content = systemPrompt },
                new Message { Role = "user", Content = prompt }
            }
        };

        var response = await SendRequestAsync(chatRequest);
        return response?.Choices?.FirstOrDefault()?.Message.Content ?? string.Empty;
    }

    public async Task<string> FillTemplateAsync(string codeTemplate, string prompt,
        string systemPrompt = "You are a helpful coding assistant.")
    {
        var templatePrompt = $@"I have a code template in C# that needs to be filled with appropriate content.

CODE TEMPLATE:
```
{codeTemplate}
```

INSTRUCTIONS:
{prompt}

Please fill in the template according to the instructions. Return only the filled template code without explanations.";

        var filledContent = await PromptAsync(templatePrompt, systemPrompt);

        // Extract just the code from the response if it contains markdown code blocks
        if (filledContent.Contains("```"))
        {
            var startIndex = filledContent.IndexOf("```");
            var endIndex = filledContent.LastIndexOf("```");

            if (startIndex >= 0 && endIndex > startIndex)
            {
                // Find the first newline after opening code block
                var codeStartIndex = filledContent.IndexOf('\n', startIndex);
                if (codeStartIndex > 0)
                {
                    // Extract code between opening and closing blocks
                    return filledContent.Substring(codeStartIndex + 1, endIndex - codeStartIndex - 1).Trim();
                }
            }
        }

        return filledContent;
    }

    public async Task<Dictionary<string, string>> FillMultipleTemplatesAsync(
        Dictionary<string, string> templates,
        string prompt,
        string systemPrompt = "You are a helpful coding assistant.")
    {
        // Build a combined prompt with all templates
        var templatePromptBuilder = new StringBuilder();
        templatePromptBuilder.AppendLine("I have multiple code templates in C# that need to be filled with appropriate content. The templates are related and should be consistent with each other.");
        templatePromptBuilder.AppendLine();
        
        foreach (var (name, template) in templates)
        {
            templatePromptBuilder.AppendLine($"TEMPLATE '{name}':");
            templatePromptBuilder.AppendLine("```");
            templatePromptBuilder.AppendLine(template);
            templatePromptBuilder.AppendLine("```");
            templatePromptBuilder.AppendLine();
        }
        
        templatePromptBuilder.AppendLine("INSTRUCTIONS:");
        templatePromptBuilder.AppendLine(prompt);
        templatePromptBuilder.AppendLine();
        templatePromptBuilder.AppendLine("Please fill in all templates according to the instructions. Format your answer in a json file like this: ");
        templatePromptBuilder.AppendLine("");
        
        foreach (var name in templates.Keys)
        {
            templatePromptBuilder.AppendLine($"\"{name}\":");
            templatePromptBuilder.AppendLine("```");
            templatePromptBuilder.AppendLine("<filled code here>");
            templatePromptBuilder.AppendLine("```");
            templatePromptBuilder.AppendLine();
        }

        var response = await PromptAsync(templatePromptBuilder.ToString(), systemPrompt);

        Console.WriteLine(templatePromptBuilder);
        Console.WriteLine(response);
        
        // Parse the response to extract each filled template
        var result = new Dictionary<string, string>();
        
        foreach (var name in templates.Keys)
        {
            var pattern = $@"FILLED TEMPLATE '{Regex.Escape(name)}':\s*```(.*?)```";
            var match = Regex.Match(response, pattern, RegexOptions.Singleline);
            
            if (match.Success && match.Groups.Count > 1)
            {
                // Extract the content between the code block markers
                var code = match.Groups[1].Value.Trim();
                result[name] = code;
            }
            else
            {
                // If no match found, return the original template
                result[name] = templates[name];
            }
        }
        
        return result;
    }

    private async Task<ChatResponse> SendRequestAsync(ChatRequest chatRequest)
    {
        var requestJson = JsonSerializer.Serialize(chatRequest, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        var content = new StringContent(requestJson, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(_endpoint, content);
        response.EnsureSuccessStatusCode();

        var responseJson = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<ChatResponse>(responseJson, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        })!;
    }
}
