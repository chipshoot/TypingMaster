using Microsoft.AspNetCore.Components;
using TypingMaster.Client.Services;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Shared.Utility;

namespace TypingMaster.Client;

public class ApplicationContext(
    IClientStorageService storage,
    NavigationManager navigationManager,
    ICourseWebService courseWebService)
{
    private const string AccountKey = "Account";
    private const string CourseKey = "Course";
    private const string IsLoggedKey = "Login";
    private const string TokenKey = "Token";
    private const string RefreshTokenKey = "RefreshToken";
    private Account? _currentAccount;
    private Guid _currentCourseId;
    private CourseDto? _currentCourse;
    private bool _isLoggedIn;
    private string? _token;
    private string? _refreshToken;

    public bool IsLoggedIn
    {
        get => _isLoggedIn;
        set
        {
            if (_isLoggedIn != value)
            {
                _isLoggedIn = value;
                SaveContext(IsLoggedKey);
                NotifyStateChanged();
            }
        }
    }

    public string? Token
    {
        get => _token;
        set
        {
            if (_token != value)
            {
                _token = value;
                if (value != null)
                {
                    storage.SetItem(TokenKey, value);
                }
                else
                {
                    storage.RemoveItem(TokenKey);
                }
            }
        }
    }

    // Add refresh token property
    public string? RefreshToken
    {
        get => _refreshToken;
        set
        {
            if (_refreshToken != value)
            {
                _refreshToken = value;
                if (value != null)
                {
                    storage.SetItem(RefreshTokenKey, value);
                }
                else
                {
                    storage.RemoveItem(RefreshTokenKey);
                }
            }
        }
    }

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set
        {
            if (_currentAccount != value)
            {
                _currentAccount = value;
                SaveContext(AccountKey);
                NotifyStateChanged();
            }
        }
    }

    public CourseDto? CurrentCourse
    {
        get => _currentCourse;
        set
        {
            if (_currentCourse != value)
            {
                _currentCourse = value;
                SaveContext(CourseKey);
                NotifyStateChanged();
            }
        }
    }

    public async Task GetCourse()
    {
        if (_currentCourseId == Guid.Empty)
        {
            return;
        }
        _currentCourse = await courseWebService.GetCourse(_currentCourseId);
    }

    public void InitializeAccount()
    {
        _currentAccount = storage.GetItem<Account>(AccountKey);
        _currentCourseId = storage.GetItem<Guid>(CourseKey);
        IsLoggedIn = storage.GetItem<bool>(IsLoggedKey);
        _token = storage.GetItem<string>(TokenKey);
        _refreshToken = storage.GetItem<string>(RefreshTokenKey);

        if (_currentAccount == null || _currentCourseId == Guid.Empty)
        {
            return;
        }

        NotifyStateChanged();
    }

    public void ClearContext()
    {
        _currentAccount = null;
        _currentCourse = null;
        IsLoggedIn = false;
        _token = null;
        storage.RemoveItem(AccountKey);
        storage.RemoveItem(CourseKey);
        storage.RemoveItem(IsLoggedKey);
        storage.RemoveItem(TokenKey);
        storage.RemoveItem(RefreshTokenKey);
        NotifyStateChanged();
    }

    private void SaveContext(string key)
    {
        if (_currentAccount != null && key == AccountKey)
        {
            storage.SetItem(AccountKey, _currentAccount);
        }

        if (_currentCourse != null && key == CourseKey)
        {
            storage.SetItem(CourseKey, _currentCourse.Id);
        }

        if (key == IsLoggedKey)
        {
            storage.SetItem(IsLoggedKey, IsLoggedIn);
        }

        if (key == TokenKey)
        {
            if (_token != null)
            {
                storage.SetItem(TokenKey, _token);
            }
            else
            {
                storage.RemoveItem(TokenKey);
            }
        }

        if (key == RefreshTokenKey)
        {
            if (_refreshToken != null)
            {
                storage.SetItem(RefreshTokenKey, _refreshToken);
            }
            else
            {
                storage.RemoveItem(RefreshTokenKey);
            }
        }
    }

    /// <summary>
    /// Ensures the user is authenticated. If not, redirects to login page.
    /// </summary>
    /// <param name="returnUrl">Optional URL to return to after login</param>
    /// <returns>True if authenticated, false if redirected</returns>
    public bool EnsureAuthenticated(string? returnUrl = null)
    {
        // Refresh authentication state from storage
        InitializeAccount();

        if (!IsLoggedIn || CurrentAccount == null)
        {
            // Redirect to login page with return URL if provided
            if (!string.IsNullOrEmpty(returnUrl))
            {
                navigationManager.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
            else
            {
                navigationManager.NavigateTo("/login");
            }

            return false;
        }

        return true;
    }

    /// <summary>
    /// Asynchronously ensures authentication status is current and redirects if needed
    /// </summary>
    /// <param name="returnUrl">Optional URL to return to after login</param>
    public Task<bool> EnsureAuthenticatedAsync(string? returnUrl = null)
    {
        return Task.FromResult(EnsureAuthenticated(returnUrl));
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}