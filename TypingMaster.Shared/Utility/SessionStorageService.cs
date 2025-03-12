using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace TypingMaster.Shared.Utility;

public class SessionStorageService(IServiceProvider serviceProvider, ILogger logger) : IClientStorageService
{
    public void SetItem<T>(string key, T value)
    {

        try
        {
            var scope = serviceProvider.CreateScope();
            var localStorage =scope.ServiceProvider.GetRequiredService<Blazored.LocalStorage.ISyncLocalStorageService>();
            localStorage.SetItem<T>(key, value);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error happened when try to set item to local storage");
            throw;
        }
    }

    public T GetItem<T>(string key)
    {
        try
        {

            var scope = serviceProvider.CreateScope();
            var localStorage = scope.ServiceProvider.GetRequiredService<Blazored.LocalStorage.ISyncLocalStorageService>();
            var result = localStorage.GetItem<T>(key);
            return result;
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error happened when try to get item to local storage");
            throw;
        }
    }

    public void RemoveItem(string key)
    {
        try
        {
            var scope = serviceProvider.CreateScope();
            var localStorage =
                scope.ServiceProvider.GetRequiredService<Blazored.LocalStorage.ISyncLocalStorageService>();
            localStorage.RemoveItem(key);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error happened when try to remove item from local storage");
            throw;
        }
    }
}