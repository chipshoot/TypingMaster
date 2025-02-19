﻿using TypingMaster.Business.Models;

namespace TypingMaster.Business.Contract
{
    public interface ITypingTrainer
    {
        string GetPracticeText(int lessonId);

        void CheckPracticeResult(int courseId, int lessonId, TypingStats stats);
    }
}