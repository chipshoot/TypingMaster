namespace TypingMaster.Business.Models;

public class Lesson
{
    public int Id { get; set; }

    // The target key or works to practice
    public IEnumerable<string> Target { get; set; } = [];

    public string PracticeText { get; set; } = null!;

    public int Point { get; set; }

    public string? Description { get; set; }
}