using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business;
using TypingMaster.Client;
using TypingMaster.Shared.Utility;
using Microsoft.AspNetCore.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.BrowserConsole()
    .WriteTo.Seq("http://localhost:8081")
    .CreateLogger();

builder.Logging.AddSerilog(Log.Logger);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<IClientStorageService, SessionStorageService>();
builder.Services.AddSingleton<ApplicationContext>(sp => 
    new ApplicationContext(
        sp.GetRequiredService<IClientStorageService>(),
        sp.GetRequiredService<NavigationManager>()
    )
);
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ITypingTrainer, TypingTrainer>();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBootstrapBlazor();


builder.Services.AddSingleton(Log.Logger);
await builder.Build().RunAsync();
