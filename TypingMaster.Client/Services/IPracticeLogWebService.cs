using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public interface IPracticeLogWebService
    {
        Task<PagedResult<DrillStats>> GetPaginatedDrillStatsAsync( int practiceLogId, int page = 1, int pageSize = 10, bool sortByNewest = true);
    }
}
