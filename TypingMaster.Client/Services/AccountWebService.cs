using System.Net.Http.Json;
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public class AccountWebService(HttpClient httpClient, IApiConfiguration apiConfig) : IAccountWebService
    {
        private const string BaseUrl = "api/account";

        public async Task<Account?> GetAccountAsync(int accountId)
        {
            try
            {
                var url = apiConfig.BuildApiUrl($"{BaseUrl}/{accountId}");
                return await httpClient.GetFromJsonAsync<Account>(url);
            }
            catch (Exception)
            {
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
            catch (Exception)
            {
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
            catch (Exception)
            {
                return null;
            }
        }
    }
}
