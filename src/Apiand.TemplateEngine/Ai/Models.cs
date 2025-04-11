using System.Text.Json.Serialization;

namespace Apiand.TemplateEngine.Ai;

public class ChatRequest
{
    public string Model { get; set; }
    public List<Message> Messages { get; set; }
}

public class Message
{
    public string Role { get; set; }
    public string Content { get; set; }
}

public class ChatResponse
{
    public string Id { get; set; }
    public string Object { get; set; }
    public long Created { get; set; }
    public string Model { get; set; }
    public List<Choice> Choices { get; set; }
    public Usage Usage { get; set; }
}

public class Choice
{
    public Message Message { get; set; }
    public string FinishReason { get; set; }
    public int Index { get; set; }
}

public class Usage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; set; }
    
    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; set; }
    
    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; set; }
}
