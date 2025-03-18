using System.Text.Json.Serialization;

namespace TypingMaster.Business.Models.LessonData;

public class BeginnerCourseLessonData
{
    [JsonPropertyName("lessonInstructions")]
    public Dictionary<string, LessonInstruction> LessonInstructions { get; set; } = new();

    [JsonPropertyName("lessons")]
    public List<LessonData> Lessons { get; set; } = new();
}