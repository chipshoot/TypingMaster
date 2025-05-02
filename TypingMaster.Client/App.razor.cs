using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using TypingMaster.Client.Services;
using TypingMaster.Core.Models;
using TypingMaster.Shared.Utility;

namespace TypingMaster.Client;

public partial class App
{
    [Inject]
    private IClientStorageService Storage { get; set; } = default!;

    [Inject]
    private ApplicationContext AppState { get; set; } = default!;

    [Inject]
    private IAuthWebService AuthService { get; set; } = default!;

    [Inject]
    private ILogger<App> Logger { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    protected override async Task OnInitializedAsync()
    {
        // Check if RememberMe is set before clearing tokens
        var rememberMe = Storage.GetItem<bool>("RememberMe");

        if (rememberMe)
        {
            // Try to perform auto-login with stored tokens
            var token = Storage.GetItem<string>("Token");
            var refreshToken = Storage.GetItem<string>("RefreshToken");

            Logger.LogInformation("Remember Me is enabled, checking for saved tokens");

            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(refreshToken))
            {
                try
                {
                    Logger.LogInformation("Found saved tokens, attempting auto-login");

                    // Try to auto-login with saved tokens
                    var success = await AuthService.TryAutoLoginAsync(token, refreshToken);
                    if (success)
                    {
                        Logger.LogInformation("Auto-login successful");
                        return; // Skip token clearing
                    }

                    // Check if auto-login failed due to missing email (which would result in a failed refresh token)
                    var accountEmail = AppState.CurrentAccount?.AccountEmail;
                    if (string.IsNullOrEmpty(accountEmail))
                    {
                        Logger.LogWarning("Auto-login failed: Email not found in account data");
                        // Clear any partial login data
                        Storage.RemoveItem("Account");
                        Storage.RemoveItem("Token");
                        Storage.RemoveItem("RefreshToken");

                        // Redirect to login page with error message
                        Navigation.NavigateTo("/login?error=Invalid+account+data.+Please+log+in+again.");
                        return;
                    }

                    Logger.LogWarning("Auto-login failed despite having tokens");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Error during auto-login");
                }
            }
            else
            {
                Logger.LogWarning("Remember Me is enabled but tokens are missing or incomplete");
            }
        }

        // Always clear these items on start if auto-login failed or was not attempted
        Storage.RemoveItem("Account");
        Storage.RemoveItem("Course");
        Storage.RemoveItem("Login");

        // Only clear tokens if RememberMe is false
        if (!rememberMe)
        {
            Storage.RemoveItem("Token");
            Storage.RemoveItem("RefreshToken");
            Storage.RemoveItem("RememberMe");
        }
    }
}