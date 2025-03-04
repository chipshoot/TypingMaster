using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;

namespace TypingMaster.Client;

public class TypingTrainerContext
{
    private Account? _currentAccount;
    private ICourse? _currentCourse;

    public bool IsLoggedIn { get; set; }

    public Account? CurrentAccount
    {
        get => _currentAccount;
        set
        {
            if (_currentAccount != value)
            {
                _currentAccount = value;
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
                NotifyStateChanged();
            }
        }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
}