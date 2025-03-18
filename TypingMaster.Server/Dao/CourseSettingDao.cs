namespace TypingMaster.Server.Dao;

public class CourseSettingDao
{
    public int Minutes { get; set; }

    public StatsDao TargetStats { get; set; } 

    public int NewKeysPerStep { get; set; }
    
    public int PracticeTextLength { get; set; }
}