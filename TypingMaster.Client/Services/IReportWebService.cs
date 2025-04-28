using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;

namespace TypingMaster.Client.Services
{
    public interface IReportWebService
    {
        /// <summary>
        /// Gets the key labels from the practice history
        /// </summary>
        /// <param name="history">The practice log containing key statistics</param>
        /// <returns>A collection of key labels</returns>
        Task<IEnumerable<string>> GetKeyLabels(PracticeLog history);

        /// <summary>
        /// Gets the key statistics from the practice history
        /// </summary>
        /// <param name="history">The practice log containing key statistics</param>
        /// <param name="includeLastSession">Whether to include statistics from the last practice session</param>
        /// <returns>A dictionary of key statistics by type (typeSpeed, latency, accuracy)</returns>
        Task<Dictionary<string, IEnumerable<double>>> GetKeyStats(PracticeLog history, bool includeLastSession);

        /// <summary>
        /// Gets the progress records for a specific course and training type
        /// </summary>
        /// <param name="history">The practice log containing practice statistics</param>
        /// <param name="course">The course to get progress records for</param>
        /// <param name="type">The training type to filter progress records by</param>
        /// <param name="page">The page number to retrieve (1-based)</param>
        /// <param name="pageSize">The number of records per page</param>
        /// <param name="sortByNewest">Whether to sort records by newest first</param>
        /// <returns>A paged result containing progress records</returns>
        Task<PagedResult<ProgressRecord>> GetProgressRecords(
            PracticeLog history,
            CourseDto course,
            TrainingType type,
            int page = 1,
            int pageSize = 10,
            bool sortByNewest = true);
    }
}
