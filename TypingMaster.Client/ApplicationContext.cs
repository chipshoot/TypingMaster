using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using TypingMaster.Client.Services;
using TypingMaster.Core.Constants;
using TypingMaster.Core.Models;
using TypingMaster.Core.Models.Courses;
using TypingMaster.Shared.Utility;

namespace TypingMaster.Client;

public class ApplicationContext
{
    private readonly IClientStorageService _storage;
    private readonly NavigationManager _navigationManager;
    private readonly ICourseWebService _courseWebService;

    private const string AccountKey = "Account";
    private const string CourseKey = "Course";
    private const string IsLoggedKey = "Login";
    private const string TokenKey = "Token";
    private const string RefreshTokenKey = "RefreshToken";
    private const string RememberMeKey = "RememberMe";
    private const string UsernameKey = "Username";
    private const string AccountEmailKey = "AccountEmail";
    private const string PracticePhaseKey = "PracticePhase";
    private Account? _currentAccount;
    private Guid _currentCourseId;
    private CourseDto? _currentCourse;
    private PracticePhases _currentPracticePhase = PracticePhases.NotSet;
    private bool _isLoggedIn;
    private string? _token;
    private string? _refreshToken;
    private string? _accountEmail;
    private bool _rememberMe;

    public ApplicationContext(
        IClientStorageService storage,
        NavigationManager navigationManager,
        ICourseWebService courseWebService)
    {
        _storage = storage;
        _navigationManager = navigationManager;
        _courseWebService = courseWebService;

        // Register for navigation events
        _navigationManager.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        // Invoke the OnNavigating event when navigation occurs
        OnNavigating?.Invoke();
    }

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
                    _storage.SetItem(TokenKey, value);
                }
                else
                {
                    _storage.RemoveItem(TokenKey);
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
                    _storage.SetItem(RefreshTokenKey, value);
                }
                else
                {
                    _storage.RemoveItem(RefreshTokenKey);
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

    public PracticePhases CurrentPracticePhase
    {
        get => _currentPracticePhase;
        set
        {
            if (_currentPracticePhase != value)
            {
                _currentPracticePhase = value;
                SaveContext(PracticePhaseKey);
                NotifyStateChanged();
            }
        }
    }

    public int GetLineWidth()
    {
        if (!IsLoggedIn || CurrentAccount == null)
        {
            return TypingMasterConstants.DefaultTypingWindowWidth;
        }

        return CurrentAccount.Settings.TryGetValue("TypingWindowWidth", out var typingWindowWidth)
            ? typingWindowWidth.ConvertToInt(TypingMasterConstants.DefaultTypingWindowWidth)
            : TypingMasterConstants.DefaultTypingWindowWidth;
    }

    public bool RememberMe
    {
        get => _rememberMe;
        private set
        {
            if (_rememberMe != value)
            {
                _rememberMe = value;
                SaveContext(RememberMeKey);
            }
        }
    }

    public async Task GetCourse()
    {
        if (_currentCourseId == Guid.Empty)
        {
            return;
        }
        _currentCourse = await _courseWebService.GetCourse(_currentCourseId);
    }

    public void InitializeAccount()
    {
        _currentAccount = _storage.GetItem<Account>(AccountKey);
        _currentCourseId = _storage.GetItem<Guid>(CourseKey);
        IsLoggedIn = _storage.GetItem<bool>(IsLoggedKey);
        _token = _storage.GetItem<string>(TokenKey);
        _refreshToken = _storage.GetItem<string>(RefreshTokenKey);
        _rememberMe = _storage.GetItem<bool>(RememberMeKey);

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
        _refreshToken = null;

        _storage.RemoveItem(AccountKey);
        _storage.RemoveItem(CourseKey);
        _storage.RemoveItem(IsLoggedKey);

        // Only remove tokens if RememberMe is false
        if (!_rememberMe)
        {
            _storage.RemoveItem(TokenKey);
            _storage.RemoveItem(RefreshTokenKey);
            _storage.RemoveItem(RememberMeKey);
            _storage.RemoveItem(UsernameKey);
            _rememberMe = false;
        }

        NotifyStateChanged();
    }

    private void SaveContext(string key)
    {
        if (_currentAccount != null && key == AccountKey)
        {
            _storage.SetItem(AccountKey, _currentAccount);
        }

        if (_currentCourse != null && key == CourseKey)
        {
            _storage.SetItem(CourseKey, _currentCourse.Id);
        }

        if (key == IsLoggedKey)
        {
            _storage.SetItem(IsLoggedKey, IsLoggedIn);
        }

        if (key == TokenKey)
        {
            if (_token != null)
            {
                _storage.SetItem(TokenKey, _token);
            }
            else
            {
                _storage.RemoveItem(TokenKey);
            }
        }

        if (key == RefreshTokenKey)
        {
            if (_refreshToken != null)
            {
                _storage.SetItem(RefreshTokenKey, _refreshToken);
            }
            else
            {
                _storage.RemoveItem(RefreshTokenKey);
            }
        }

        if (key == AccountEmailKey)
        {
            _storage.SetItem(AccountEmailKey, _accountEmail);
        }

        if (key == RememberMeKey)
        {
            _storage.SetItem(RememberMeKey, _rememberMe);
        }

        if (key == PracticePhaseKey)
        {
            _storage.SetItem(PracticePhaseKey, (int)_currentPracticePhase);
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
                _navigationManager.NavigateTo($"/login?returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
            else
            {
                _navigationManager.NavigateTo("/login");
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

    public void SetRememberMe(bool remember)
    {
        RememberMe = remember;
    }

    public void SetAccountEmail(string email)
    {
        if (_accountEmail != email)
        {
            _accountEmail = email;
            _storage.SetItem(AccountEmailKey, email);
        }
    }

    public event Action? OnChange;

    // Add a new event that fires when navigating away from a page
    public event Action? OnNavigating;

    private void NotifyStateChanged() => OnChange?.Invoke();

    public void SaveUserName(string userName)
    {
        if (!string.IsNullOrEmpty(userName))
        {
            _storage.SetItem(UsernameKey, userName);
        }
    }

    public string? GetStoredAccountEmail()
    {
        return _storage.GetItem<string>(AccountEmailKey);
    }
}