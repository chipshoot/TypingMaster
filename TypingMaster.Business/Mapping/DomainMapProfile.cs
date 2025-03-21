﻿using AutoMapper;
using TypingMaster.Business.Models;
using TypingMaster.Business.Models.Courses;
using TypingMaster.Business.Contract;
using TypingMaster.DataAccess.Dao;

namespace TypingMaster.Business.Mapping
{
    public class DomainMapProfile : Profile
    {
        public DomainMapProfile()
        {
            // Add Queue mapping configuration at the top of the constructor
            CreateMap<Queue<KeyEventDao>, Queue<KeyEvent>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var queue = new Queue<KeyEvent>();
                    foreach (var item in src)
                    {
                        queue.Enqueue(context.Mapper.Map<KeyEvent>(item));
                    }
                    return queue;
                });

            CreateMap<Queue<KeyEvent>, Queue<KeyEventDao>>()
                .ConvertUsing((src, dest, context) =>
                {
                    var queue = new Queue<KeyEventDao>();
                    foreach (var item in src)
                    {
                        queue.Enqueue(context.Mapper.Map<KeyEventDao>(item));
                    }
                    return queue;
                });

            // Map from CourseDao to CourseBase
            CreateMap<CourseDao, BeginnerCourse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.SettingsJson))
                .ForMember(dest => dest.LessonDataUrl, opt => opt.MapFrom(src => src.LessonDataUrl));

            CreateMap<CourseDao, AdvancedLevelCourse>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.SettingsJson))
                .ForMember(dest => dest.LessonDataUrl, opt => opt.MapFrom(src => src.LessonDataUrl));

            // Map from ICourse implementations to CourseDao
            CreateMap<ICourse, CourseDao>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ((int)src.Type).ToString()))
                .ForMember(dest => dest.SettingsJson, opt => opt.MapFrom((src, dest, member, context) =>
                    src.Settings == null ? null : new CourseSettingDao
                    {
                        Minutes = src.Settings.Minutes,
                        NewKeysPerStep = src.Settings.NewKeysPerStep,
                        PracticeTextLength = src.Settings.PracticeTextLength,
                        TargetStats = new StatsDao
                        {
                            Wpm = src.Settings.TargetStats.Wpm,
                            Accuracy = src.Settings.TargetStats.Accuracy
                        }
                    }))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.LessonDataUrl, opt => opt.MapFrom(src => src.LessonDataUrl))
                .ForMember(dest => dest.AccountId, opt => opt.Ignore());

            // Map from AccountDao to Account
            CreateMap<AccountDao, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.AccountName))
                .ForMember(dest => dest.AccountEmail, opt => opt.MapFrom(src => src.AccountEmail))
                .ForMember(dest => dest.GoalStats, opt => opt.MapFrom(src => src.GoalStats))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.History))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src =>
                    src.Courses.Where(c => c.Type == ((int)TrainingType.Course).ToString()).Select(c => c.Id).FirstOrDefault()))
                .ForMember(dest => dest.TestCourseId, opt => opt.MapFrom(src =>
                    src.Courses.Where(c => c.Type == ((int)TrainingType.AllKeysTest).ToString()).Select(c => c.Id).FirstOrDefault()))
                .ForMember(dest => dest.GameCourseId, opt => opt.MapFrom(src =>
                    src.Courses.Where(c => c.Type == ((int)TrainingType.Game).ToString()).Select(c => c.Id).FirstOrDefault()));

            // Map from StatsDao to StatsBase
            CreateMap<StatsDao, StatsBase>()
                .ForMember(dest => dest.Wpm, opt => opt.MapFrom(src => src.Wpm))
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.Accuracy));

            // Map from StatsBase to StatsDao (reverse mapping)
            CreateMap<StatsBase, StatsDao>()
                .ForMember(dest => dest.Wpm, opt => opt.MapFrom(src => src.Wpm))
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.Accuracy));

            // Map from CourseSettingDao to CourseSetting
            CreateMap<CourseSettingDao, CourseSetting>()
                .ForMember(dest => dest.Minutes, opt => opt.MapFrom(src => src.Minutes))
                .ForMember(dest => dest.TargetStats, opt => opt.MapFrom(src => src.TargetStats))
                .ForMember(dest => dest.NewKeysPerStep, opt => opt.MapFrom(src => src.NewKeysPerStep))
                .ForMember(dest => dest.PracticeTextLength, opt => opt.MapFrom(src => src.PracticeTextLength));

            // Map from CourseSetting to CourseSettingDao (reverse mapping)
            CreateMap<CourseSetting, CourseSettingDao>()
                .ForMember(dest => dest.Minutes, opt => opt.MapFrom(src => src.Minutes))
                .ForMember(dest => dest.TargetStats, opt => opt.MapFrom(src => src.TargetStats))
                .ForMember(dest => dest.NewKeysPerStep, opt => opt.MapFrom(src => src.NewKeysPerStep))
                .ForMember(dest => dest.PracticeTextLength, opt => opt.MapFrom(src => src.PracticeTextLength));

            // Map from UserProfileDao to UserProfile
            CreateMap<UserProfileDao, UserProfile>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

            // Map from UserProfile to UserProfileDao (reverse mapping)
            CreateMap<UserProfile, UserProfileDao>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

            // Map from KeyEventDao to KeyEvent
            CreateMap<KeyEventDao, KeyEvent>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.TypedKey, opt => opt.MapFrom(src => src.TypedKey))
                .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect))
                .ForMember(dest => dest.KeyDownTime, opt => opt.MapFrom(src => src.KeyDownTime))
                .ForMember(dest => dest.KeyUpTime, opt => opt.MapFrom(src => src.KeyUpTime))
                .ForMember(dest => dest.Latency, opt => opt.MapFrom(src => src.Latency));

            // Map from KeyEvent to KeyEventDao (reverse mapping)
            CreateMap<KeyEvent, KeyEventDao>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.TypedKey, opt => opt.MapFrom(src => src.TypedKey))
                .ForMember(dest => dest.IsCorrect, opt => opt.MapFrom(src => src.IsCorrect))
                .ForMember(dest => dest.KeyDownTime, opt => opt.MapFrom(src => src.KeyDownTime))
                .ForMember(dest => dest.KeyUpTime, opt => opt.MapFrom(src => src.KeyUpTime))
                .ForMember(dest => dest.Latency, opt => opt.MapFrom(src => src.Latency));

            // Map from KeyStatsDao to KeyStats
            CreateMap<KeyStatsDao, KeyStats>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.TypingCount, opt => opt.MapFrom(src => src.TypingCount))
                .ForMember(dest => dest.CorrectCount, opt => opt.MapFrom(src => src.CorrectCount))
                .ForMember(dest => dest.PressDuration, opt => opt.MapFrom(src => src.PressDuration))
                .ForMember(dest => dest.Latency, opt => opt.MapFrom(src => src.Latency))
                .ForMember(dest => dest.Wpm, opt => opt.MapFrom(src => src.Wpm))
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.Accuracy));

            // Map from KeyStats to KeyStatsDao (reverse mapping)
            CreateMap<KeyStats, KeyStatsDao>()
                .ForMember(dest => dest.Key, opt => opt.MapFrom(src => src.Key))
                .ForMember(dest => dest.TypingCount, opt => opt.MapFrom(src => src.TypingCount))
                .ForMember(dest => dest.CorrectCount, opt => opt.MapFrom(src => src.CorrectCount))
                .ForMember(dest => dest.PressDuration, opt => opt.MapFrom(src => src.PressDuration))
                .ForMember(dest => dest.Latency, opt => opt.MapFrom(src => src.Latency))
                .ForMember(dest => dest.Wpm, opt => opt.MapFrom(src => src.Wpm))
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.Accuracy));

            // Map from PracticeLogDao to PracticeLog
            CreateMap<PracticeLogDao, PracticeLog>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CurrentCourseId, opt => opt.MapFrom(src => src.CurrentCourseId))
                .ForMember(dest => dest.CurrentLessonId, opt => opt.MapFrom(src => src.CurrentLessonId))
                .ForMember(dest => dest.PracticeStats, opt => opt.MapFrom(src => src.PracticeStats))
                .ForMember(dest => dest.KeyStats, opt => opt.MapFrom(src => src.KeyStatsJson))
                .ForMember(dest => dest.PracticeDuration, opt => opt.MapFrom(src => src.PracticeDuration));

            // Map from PracticeLog to PracticeLogDao (reverse mapping)
            CreateMap<PracticeLog, PracticeLogDao>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CurrentCourseId, opt => opt.MapFrom(src => src.CurrentCourseId))
                .ForMember(dest => dest.CurrentLessonId, opt => opt.MapFrom(src => src.CurrentLessonId))
                .ForMember(dest => dest.PracticeStats, opt => opt.MapFrom(src => src.PracticeStats))
                .ForMember(dest => dest.KeyStatsJson, opt => opt.MapFrom(src => src.KeyStats))
                .ForMember(dest => dest.PracticeDuration, opt => opt.MapFrom(src => src.PracticeDuration));

            // Map from DrillStatsDao to DrillStats if needed
            CreateMap<DrillStatsDao, DrillStats>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PracticeLogId, opt => opt.MapFrom(src => src.PracticeLogId))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.LessonId))
                .ForMember(dest => dest.PracticeText, opt => opt.MapFrom(src => src.PracticeText))
                .ForMember(dest => dest.TypedText, opt => opt.MapFrom(src => src.TypedText))
                .ForMember(dest => dest.KeyEvents, opt => opt.MapFrom(src => src.KeyEventsJson))
                .ForMember(dest => dest.Wpm, opt => opt.MapFrom(src => src.Wpm))
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.Accuracy))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (TrainingType)src.TrainingType))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.FinishTime, opt => opt.MapFrom(src => src.FinishTime));

            // Map from DrillStats to DrillStatsDao (reverse mapping)
            CreateMap<DrillStats, DrillStatsDao>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PracticeLogId, opt => opt.MapFrom(src => src.PracticeLogId))
                .ForMember(dest => dest.CourseId, opt => opt.MapFrom(src => src.CourseId))
                .ForMember(dest => dest.LessonId, opt => opt.MapFrom(src => src.LessonId))
                .ForMember(dest => dest.PracticeText, opt => opt.MapFrom(src => src.PracticeText))
                .ForMember(dest => dest.TypedText, opt => opt.MapFrom(src => src.TypedText))
                .ForMember(dest => dest.KeyEventsJson, opt => opt.MapFrom(src => src.KeyEvents))
                .ForMember(dest => dest.Wpm, opt => opt.MapFrom(src => src.Wpm))
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.Accuracy))
                // Convert TrainingType enum to int
                .ForMember(dest => dest.TrainingType, opt => opt.MapFrom(src => (int)src.Type))
                .ForMember(dest => dest.StartTime, opt => opt.MapFrom(src => src.StartTime))
                .ForMember(dest => dest.FinishTime, opt => opt.MapFrom(src => src.FinishTime));

            // Reverse mapping (from Account to AccountDao)
            CreateMap<Account, AccountDao>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.AccountName))
                .ForMember(dest => dest.AccountEmail, opt => opt.MapFrom(src => src.AccountEmail))
                .ForMember(dest => dest.GoalStats, opt => opt.MapFrom(src => src.GoalStats))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.History))
                .ForMember(dest => dest.Courses, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        var courses = new List<CourseDao>();

                        // Regular course - add only if CourseId is not empty
                        if (src.CourseId != Guid.Empty)
                        {
                            courses.Add(new CourseDao
                            {
                                Id = src.CourseId,
                                AccountId = src.Id,
                                Type = ((int)TrainingType.Course).ToString()
                            });
                        }

                        // Test course - add only if TestCourseId is not empty
                        if (src.TestCourseId != Guid.Empty)
                        {
                            courses.Add(new CourseDao
                            {
                                Id = src.TestCourseId,
                                AccountId = src.Id,
                                Type = ((int)TrainingType.AllKeysTest).ToString()
                            });
                        }

                        // Game course - add only if GameCourseId is not empty
                        if (src.GameCourseId != Guid.Empty)
                        {
                            courses.Add(new CourseDao
                            {
                                Id = src.GameCourseId,
                                AccountId = src.Id,
                                Type = ((int)TrainingType.Game).ToString()
                            });
                        }

                        return courses;
                    });
                });

            // LoginLog mappings
            CreateMap<LoginLogDao, LoginLog>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.UserAgent, opt => opt.MapFrom(src => src.UserAgent))
                .ForMember(dest => dest.LoginTime, opt => opt.MapFrom(src => src.LoginTime))
                .ForMember(dest => dest.IsSuccessful, opt => opt.MapFrom(src => src.IsSuccessful))
                .ForMember(dest => dest.FailureReason, opt => opt.MapFrom(src => src.FailureReason));

            CreateMap<LoginLog, LoginLogDao>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.IpAddress, opt => opt.MapFrom(src => src.IpAddress))
                .ForMember(dest => dest.UserAgent, opt => opt.MapFrom(src => src.UserAgent))
                .ForMember(dest => dest.LoginTime, opt => opt.MapFrom(src => src.LoginTime))
                .ForMember(dest => dest.IsSuccessful, opt => opt.MapFrom(src => src.IsSuccessful))
                .ForMember(dest => dest.FailureReason, opt => opt.MapFrom(src => src.FailureReason));

            // LoginCredential mappings
            CreateMap<LoginCredentialDao, LoginCredential>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => src.PasswordSalt))
                .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.IsEmailConfirmed))
                .ForMember(dest => dest.ConfirmationToken, opt => opt.MapFrom(src => src.ConfirmationToken))
                .ForMember(dest => dest.ConfirmationTokenExpiry, opt => opt.MapFrom(src => src.ConfirmationTokenExpiry))
                .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken))
                .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.MapFrom(src => src.RefreshTokenExpiry))
                .ForMember(dest => dest.ResetPasswordToken, opt => opt.MapFrom(src => src.ResetPasswordToken))
                .ForMember(dest => dest.ResetPasswordTokenExpiry, opt => opt.MapFrom(src => src.ResetPasswordTokenExpiry))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
                .ForMember(dest => dest.FailedLoginAttempts, opt => opt.MapFrom(src => src.FailedLoginAttempts))
                .ForMember(dest => dest.LockoutEnd, opt => opt.MapFrom(src => src.LockoutEnd))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.ExternalIdpId, opt => opt.MapFrom(src => src.ExternalIdpId))
                .ForMember(dest => dest.ExternalIdpType, opt => opt.MapFrom(src => src.ExternalIdpType));

            CreateMap<LoginCredential, LoginCredentialDao>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountId, opt => opt.MapFrom(src => src.AccountId))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.PasswordHash))
                .ForMember(dest => dest.PasswordSalt, opt => opt.MapFrom(src => src.PasswordSalt))
                .ForMember(dest => dest.IsEmailConfirmed, opt => opt.MapFrom(src => src.IsEmailConfirmed))
                .ForMember(dest => dest.ConfirmationToken, opt => opt.MapFrom(src => src.ConfirmationToken))
                .ForMember(dest => dest.ConfirmationTokenExpiry, opt => opt.MapFrom(src => src.ConfirmationTokenExpiry))
                .ForMember(dest => dest.RefreshToken, opt => opt.MapFrom(src => src.RefreshToken))
                .ForMember(dest => dest.RefreshTokenExpiry, opt => opt.MapFrom(src => src.RefreshTokenExpiry))
                .ForMember(dest => dest.ResetPasswordToken, opt => opt.MapFrom(src => src.ResetPasswordToken))
                .ForMember(dest => dest.ResetPasswordTokenExpiry, opt => opt.MapFrom(src => src.ResetPasswordTokenExpiry))
                .ForMember(dest => dest.IsLocked, opt => opt.MapFrom(src => src.IsLocked))
                .ForMember(dest => dest.FailedLoginAttempts, opt => opt.MapFrom(src => src.FailedLoginAttempts))
                .ForMember(dest => dest.LockoutEnd, opt => opt.MapFrom(src => src.LockoutEnd))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.LastUpdated, opt => opt.MapFrom(src => src.LastUpdated))
                .ForMember(dest => dest.ExternalIdpId, opt => opt.MapFrom(src => src.ExternalIdpId))
                .ForMember(dest => dest.ExternalIdpType, opt => opt.MapFrom(src => src.ExternalIdpType))
                .ForMember(dest => dest.Account, opt => opt.Ignore());
        }
    }
}
