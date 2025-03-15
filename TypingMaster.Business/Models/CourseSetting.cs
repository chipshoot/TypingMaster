﻿namespace TypingMaster.Business.Models;

public class CourseSetting
{
    public int Minutes { get; set; }

    public StatsBase? TargetStats { get; set; } 

    public int NewKeysPerStep { get; set; }
    
    public int PracticeTextLength { get; set; }
}