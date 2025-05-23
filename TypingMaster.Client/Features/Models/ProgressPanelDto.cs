using TypingMaster.Core.Models;

namespace TypingMaster.Client.Features.Models;

public class ProgressPanelModel
{
    public int CurrentWpm { get; set; }

    public double CurrentAccuracy { get; set; }

    public StatsBase StatTarget { get; set; } = null!;

    public int LessonNumber { get; set; } = 1;

    public int TotalLessons { get; set; } = 10;
}