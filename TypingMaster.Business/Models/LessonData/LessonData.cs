using System.Text.Json.Serialization;

namespace TypingMaster.Business.Models.LessonData;

public class LessonData
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("target")]
    public List<string> Target { get; set; } = new();

    [JsonPropertyName("instructionKey")]
    public string? InstructionKey { get; set; }

    [JsonPropertyName("instructionParams")]
    public List<string>? InstructionParams { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    [JsonPropertyName("point")]
    public int Point { get; set; }
}