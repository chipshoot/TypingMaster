using System.Net.Http.Headers;
using System.Net.Http.Json;
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public class AccountWebService(HttpClient httpClient, IApiConfiguration apiConfig, ApplicationContext appContext, Serilog.ILogger logger) : IAccountWebService
    {
        public async Task<Account?> GetAccountAsync(int accountId)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AccountService}/{accountId}");

                // Add Authorization header with Bearer token if available
                if (!string.IsNullOrEmpty(appContext.Token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appContext.Token);
                }
                var response = await httpClient.GetFromJsonAsync<Account>(url);
                return response;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }

        public async Task<AccountResponse> UpdateAccountAsync(Account account)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AccountService}/{account.Id}");
        
                // Add Authorization header with Bearer token if available
                if (!string.IsNullOrEmpty(appContext.Token))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appContext.Token);
                }
        
                var response = await httpClient.PutAsJsonAsync(url, account);
        
                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AccountResponse>();
                    return result ?? new AccountResponse 
                    { 
                        Success = false, 
                        Message = "Failed to deserialize response",
                        StatusCode = System.Net.HttpStatusCode.OK
                    };
                }
                else
                {
                    // Try to read error response
                    AccountResponse? errorResponse = null;
                    try
                    {
                        errorResponse = await response.Content.ReadFromJsonAsync<AccountResponse>();
                    }
                    catch
                    {
                        // Unable to parse the error response
                    }
            
                    return errorResponse ?? new AccountResponse
                    {
                        Success = false,
                        Message = $"Server returned status code: {response.StatusCode}",
                        StatusCode = response.StatusCode
                    };
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error updating account");
                return new AccountResponse
                {
                    Success = false,
                    Message = $"Exception updating account: {ex.Message}",
                    StatusCode = System.Net.HttpStatusCode.InternalServerError
                };
            }
        }

        public async Task<Account?> GetGuestAccount()
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{apiConfig.ApiSettings.AccountService}/guest");
                return await httpClient.GetFromJsonAsync<Account>(url);
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return null;
            }
        }
    }
}
