using Microsoft.AspNetCore.Components;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.Shared.Utility;

namespace TypingMaster.Client;

public class ApplicationContext(IClientStorageService storage, NavigationManager navigationManager)
{
    private const string AccountKey = "Account";
    private const string CourseKey = "Course";
    private const string IsLoggedKey = "Login";
    private Account? _currentAccount;
    private Guid _currentCourseId;
    private ICourse? _currentCourse;
    private bool _isLoggedIn;

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

    public ICourse? CurrentCourse
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

    public async Task GetCourse(ICourseService courseService)
    {
        if (_currentCourseId == Guid.Empty)
        {
            return;
        }
        _currentCourse = await _courseWebService.GetCourse(_currentCourseId);
    }

    public void InitializeAccount()
    {
        _currentAccount = storage.GetItem<Account>(AccountKey);
        _currentCourseId = storage.GetItem<Guid>(CourseKey);
        IsLoggedIn = storage.GetItem<bool>(IsLoggedKey);
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
        storage.RemoveItem(AccountKey);
        storage.RemoveItem(CourseKey);
        storage.RemoveItem(IsLoggedKey);
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