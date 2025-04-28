using Microsoft.AspNetCore.Components;
using TypingMaster.Shared.Utility;

namespace TypingMaster.Client;

public partial class App
{
    [Inject]
    private IClientStorageService Storage { get; set; } = default!;

    protected override void OnInitialized()
    {
        // Clear local storage on application start
        Storage.RemoveItem("Account");
        Storage.RemoveItem("Course");
        Storage.RemoveItem("Login");
        Storage.RemoveItem("Token");
        Storage.RemoveItem("RefreshToken");
    }
}