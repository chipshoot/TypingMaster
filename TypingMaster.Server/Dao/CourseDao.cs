namespace TypingMaster.Server.Dao;

public class CourseDao
{
    public Guid Id { get; set; }

    public int AccountId { get; set; }

    public string Name { get; set; } = null!;

    public string LessonDataUrl { get; set; } = "";

    public string Description { get; set; } = "";

    public string Type { get; set; } = null!;

    public string Settings { get; set; } = null!;
}