namespace TypingMaster.Business.Models;

public class Lesson
{
    public int Id { get; set; }
    public string PracticeText { get; set; } = null!;
    public int Point { get; set; }
}