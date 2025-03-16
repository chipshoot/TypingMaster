using AutoMapper;
using TypingMaster.Business.Models;
using TypingMaster.Server.Dao;

namespace TypingMaster.Server.Mapping
{
    public class AccountMapProfile : Profile
    {
        public AccountMapProfile()
        {
            // Map from AccountDao to Account
            CreateMap<AccountDao, Account>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.AccountName, opt => opt.MapFrom(src => src.AccountName))
                .ForMember(dest => dest.AccountEmail, opt => opt.MapFrom(src => src.AccountEmail))
                .ForMember(dest => dest.GoalStats, opt => opt.MapFrom(src => src.GoalStats))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.History, opt => opt.MapFrom(src => src.History))
                // The Account model has CourseId, TestCourseId, and GameCourseId fields
                // which don't directly map to properties in AccountDao
                .ForMember(dest => dest.CourseId, opt => opt.Ignore())
                .ForMember(dest => dest.TestCourseId, opt => opt.Ignore())
                .ForMember(dest => dest.GameCourseId, opt => opt.Ignore());

            // Map from StatsDao to StatsBase
            CreateMap<StatsDao, StatsBase>()
                .ForMember(dest => dest.Wpm, opt => opt.MapFrom(src => src.Wpm))
                .ForMember(dest => dest.Accuracy, opt => opt.MapFrom(src => src.Accuracy));

            // Map from UserProfileDao to UserProfile
            CreateMap<UserProfileDao, UserProfile>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.AvatarUrl, opt => opt.MapFrom(src => src.AvatarUrl));

            // Map from PracticeLogDao to PracticeLog
            CreateMap<PracticeLogDao, PracticeLog>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CurrentCourseId, opt => opt.MapFrom(src => src.CurrentCourseId))
                .ForMember(dest => dest.CurrentLessonId, opt => opt.MapFrom(src => src.CurrentLessonId))
                .ForMember(dest => dest.PracticeStats, opt => opt.MapFrom(src => src.PracticeStats))
                .ForMember(dest => dest.KeyStats, opt => opt.MapFrom(src => src.KeyStatsJson))
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
                .ForMember(dest => dest.Courses, opt => opt.Ignore());
        }
    }
}
