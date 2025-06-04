using System.Net.Http.Headers;
using System.Net.Http.Json;
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public class PracticeLogWebService(HttpClient httpClient, IApiConfiguration apiConfig, ApplicationContext appContext, Serilog.ILogger logger) : IPracticeLogWebService
    {
        public async Task<PagedResult<DrillStats>> GetPaginatedDrillStatsAsync(
            int practiceLogId, 
            int page = 1, 
            int pageSize = 10,
            bool sortByNewest = true,
            TrainingType? type = null)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.PracticeLogService}/{practiceLogId}");
                var fullUrl = $"{url}/drill-stats?page={page}&pageSize={pageSize}&sortByNewest={sortByNewest}&type={type}";

                // Add Authorization header with Bearer token if available
                if (!string.IsNullOrEmpty(appContext.Token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appContext.Token);
                }

                var response = await httpClient.GetAsync(fullUrl);
        
                if (!response.IsSuccessStatusCode)
                {
                    return new PagedResult<DrillStats>
                    {
                        Success = false,
                        Message = $"Failed to retrieve drill stats: {response.StatusCode}"
                    };
                }
        
                var result = await response.Content.ReadFromJsonAsync<PagedResult<DrillStats>>();
                return result ?? new PagedResult<DrillStats> { Success = false, Message = "Failed to deserialize response" };
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error getting paginated drill stats");
                return new PagedResult<DrillStats>
                {
                    Success = false,
                    Message = $"Exception retrieving drill stats: {ex.Message}"
                };
            }
        }

        public async Task<PracticeLog?> UpdatePracticeLog(PracticeLog practiceLog)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.PracticeLogService}/{practiceLog.Id}");

                // Add Authorization header with Bearer token if available
                if (!string.IsNullOrEmpty(appContext.Token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appContext.Token);
                }

                var response = await httpClient.PutAsJsonAsync(url, practiceLog);
        
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<PracticeLog>();
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error update practice log");
                return null;
            }
        }

        public async Task<DrillStats?> AddDrillStatAsync(int practiceLogId, DrillStats drillStat)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.PracticeLogService}/{practiceLogId}/drill-stats");

                // Add Authorization header with Bearer token if available
                if (!string.IsNullOrEmpty(appContext.Token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appContext.Token);
                }

                var response = await httpClient.PostAsJsonAsync(url, drillStat);
        
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<DrillStats>();
                    return result;
                }

                return null;
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error adding drill stats");
                return null;
            }
        }
    }
}
