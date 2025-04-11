namespace TypingMaster.Core.Constants
{
    /// <summary>
    /// Contains constants used throughout the TypingMaster application
    /// </summary>
    public static class TypingMasterConstants
    {
        #region Client
        public const string DefaultPracticePrompt = "Type the line in the top box. The arrow moves to show the next key to type. At the end of the line press either space or Enter.";
        #endregion

        #region Course Names

        public const string BeginnerCourseName = "BeginnerCourse";
        public const string AdvancedLevelCourseName = "AdvancedLevelCourse";
        public const string AllKeysCourseName = "AllKeysTestCourse";
        public const string SpeedTestCourseName = "SpeedTestCourse";
        #endregion

        #region Course Descriptions
        public const string BeginnerCourseDescription =
            "Master Touch Typing from Scratch: This structured beginner's course guides you from the home row keys (a, s, d, f, j, k, l, ;) to full keyboard proficiency. Starting with your finger placement on home keys, each lesson gradually introduces new keys while reinforcing previously learned ones. Progress at your own pace through interactive exercises designed to build muscle memory, improve accuracy, and increase typing speed. Perfect for new typists or anyone looking to develop proper touch typing technique without looking at the keyboard. Track your WPM and accuracy as you transform from hunt-and-peck to confident touch typing.";
        
        public const string AdvancedCourseDescription = 
            "Perfect Your Typing Skills: The Advanced Course builds on your existing touch typing foundation to help you achieve professional-level speed and accuracy...";
        #endregion

        #region File Paths
        public const string BeginnerCourseLessonPath = "Resources/LessonData/beginner-course-lessons.json";
        public const string AdvancedCourseLessonPath = "Resources/LessonData/advanced-course-lessons.json";
        #endregion

        #region Error Messages
        public const string LessonNotFoundError = "Cannot found lesson";
        public const string TargetStatsNotSetError = "Target stats not set.";
        public const string NoCommonWordsError = "No common words found for the target keys: {0}";
        #endregion
        
        #region Default Values
        public const string DefaultCompleteText = "";
        #endregion
    }
}
