namespace TypingMaster.DataAccess.Dao;

public class CourseSettingDao
{
    public int Minutes { get; set; }

    public StatsDao TargetStats { get; set; } = null!;

    public int NewKeysPerStep { get; set; }
    
    public int PracticeTextLength { get; set; }
}