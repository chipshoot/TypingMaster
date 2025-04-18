
using System.Text.Json.Serialization;

namespace TypingMaster.Business.Course
{
    public class LessonData
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("target")]
        public List<string> Target { get; set; } = [];

        [JsonPropertyName("practiceText")]
        public string PracticeText { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("instruction")]
        public string Instruction { get; set; } = string.Empty;

        [JsonPropertyName("point")]
        public int Point { get; set; }
    }
}