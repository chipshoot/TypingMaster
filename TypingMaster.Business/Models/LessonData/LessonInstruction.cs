using System.Text.Json.Serialization;

namespace TypingMaster.Business.Models.LessonData;

public class LessonInstruction
{
    [JsonPropertyName("instruction")]
    public string Instruction { get; set; } = string.Empty;

    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;
}