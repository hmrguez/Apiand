using System.Text;

namespace Apiand.TemplateEngine;

public class TemplateEngine
{
    private readonly Dictionary<string, ParsedTemplate> _templateCache = new();
    
    public string Render(string templateContent, Dictionary<string, string> data)
    {
        var parsed = Parse(templateContent);
        return RenderTemplate(parsed, data);
    }
    
    public string RenderFromFile(string filePath, Dictionary<string, string> data)
    {
        if (!_templateCache.TryGetValue(filePath, out var template))
        {
            var content = File.ReadAllText(filePath);
            template = Parse(content);
            _templateCache[filePath] = template;
        }
        
        return RenderTemplate(template, data);
    }
    
    private ParsedTemplate Parse(string templateContent)
    {
        const string marker = "XXX";
        var segments = new List<TemplateSegment>();
        int currentPos = 0;
    
        while (currentPos < templateContent.Length)
        {
            int openMarkerPos = templateContent.IndexOf(marker, currentPos, StringComparison.Ordinal);
            if (openMarkerPos == -1)
            {
                // Add remaining text as literal
                segments.Add(new TemplateSegment
                {
                    Type = SegmentType.Literal,
                    Content = templateContent.Substring(currentPos)
                });
                break;
            }
    
            // Add text before placeholder as literal
            if (openMarkerPos > currentPos)
            {
                segments.Add(new TemplateSegment
                {
                    Type = SegmentType.Literal,
                    Content = templateContent.Substring(currentPos, openMarkerPos - currentPos)
                });
            }
    
            int closeMarkerPos = templateContent.IndexOf(marker, openMarkerPos + marker.Length, StringComparison.Ordinal);
            if (closeMarkerPos == -1)
            {
                // No closing markers, treat as literal
                segments.Add(new TemplateSegment
                {
                    Type = SegmentType.Literal,
                    Content = templateContent.Substring(currentPos)
                });
                break;
            }
    
            // Extract variable name
            string varName = templateContent.Substring(openMarkerPos + marker.Length, closeMarkerPos - openMarkerPos - marker.Length).Trim();
            segments.Add(new TemplateSegment
            {
                Type = SegmentType.Variable,
                Content = varName
            });
    
            currentPos = closeMarkerPos + marker.Length;
        }
    
        return new ParsedTemplate { Segments = segments };
    }
    
    private string RenderTemplate(ParsedTemplate template, Dictionary<string, string> properties)
    {
        var result = new StringBuilder();
            
        foreach (var segment in template.Segments)
        {
            if (segment.Type == SegmentType.Literal)
            {
                result.Append(segment.Content);
            }
            else if (segment.Type == SegmentType.Variable)
            {
                if (properties.TryGetValue(segment.Content, out var property))
                {
                    result.Append(property ?? string.Empty);
                }
            }
        }
        
        return result.ToString();
    }
}

public class ParsedTemplate
{
    public List<TemplateSegment> Segments { get; set; } = new();
}

public class TemplateSegment
{
    public SegmentType Type { get; set; }
    public string Content { get; set; } = string.Empty;
}

public enum SegmentType
{
    Literal,
    Variable
}