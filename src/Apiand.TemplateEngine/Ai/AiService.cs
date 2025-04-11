using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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