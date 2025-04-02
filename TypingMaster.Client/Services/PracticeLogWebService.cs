using System.Net.Http.Headers;
using System;
using System.Net.Http.Json;
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public class PracticeLogWebService(HttpClient httpClient, IApiConfiguration apiConfig, ApplicationContext appContext, Serilog.ILogger logger) : IPracticeLogWebService
    {
        private const string BaseUrl = "api/practicelog";

        public async Task<PagedResult<DrillStats>> GetPaginatedDrillStatsAsync(
            int practiceLogId, 
            int page = 1, 
            int pageSize = 10,
            bool sortByNewest = true)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{BaseUrl}/{practiceLogId}/drill-stats?page={page}&pageSize={pageSize}&sortByNewest={sortByNewest}");

                // Add Authorization header with Bearer token if available
                if (!string.IsNullOrEmpty(appContext.Token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appContext.Token);
                }

                var response = await httpClient.GetAsync(url);
        
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
    }
}
