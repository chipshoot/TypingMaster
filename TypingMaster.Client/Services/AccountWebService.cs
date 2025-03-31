using System.Net.Http.Headers;
using System;
using System.Net.Http.Json;
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public class AccountWebService(HttpClient httpClient, IApiConfiguration apiConfig, ApplicationContext appContext, Serilog.ILogger logger) : IAccountWebService
    {
        private const string BaseUrl = "api/account";

        public async Task<Account?> GetAccountAsync(int accountId)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{BaseUrl}/{accountId}");

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

        public async Task<bool> UpdateAccountAsync(Account account)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{BaseUrl}/{account.Id}");
                var response = await httpClient.PutAsJsonAsync(url, account);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return false;
            }
        }

        public async Task<Account?> GetGuestAccount()
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{BaseUrl}/-1");
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
