namespace TypingMaster.Core.Models.Courses;

public class CourseDto
{
    public Guid Id { get; set; }

    public int AccountId { get; set; }

    public string Name { get; set; } = null!;

    public TrainingType Type { get; set; }

    public string LessonDataUrl { get; set; } = string.Empty;

    public IEnumerable<Lesson> Lessons { get; set; } = [];

    public CourseSetting? Settings { get; set; }

    public string? Description { get; set; }
}