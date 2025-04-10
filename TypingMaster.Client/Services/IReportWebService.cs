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
        /// <param name="types">The training type to filter progress records by</param>
        /// <returns>A collection of progress records</returns>
        Task<IEnumerable<ProgressRecord>> GetProgressRecords(PracticeLog history, CourseDto course, params TrainingType[] types);
    }
}
