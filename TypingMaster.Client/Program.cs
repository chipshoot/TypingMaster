using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using TypingMaster.Client;
using TypingMaster.Client.Services;
using TypingMaster.Shared.Utility;

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

// Configure API settings
var apiSettings = new ApiSettings();
builder.Configuration.GetSection("ApiSettings").Bind(apiSettings);
builder.Services.AddSingleton(apiSettings);
builder.Services.AddSingleton<IApiConfiguration, ApiConfiguration>();

builder.Services.AddBlazoredLocalStorage();
builder.Services.AddSingleton<IClientStorageService, SessionStorageService>();
builder.Services.AddScoped<IAuthWebService, AuthWebService>();
builder.Services.AddScoped<IAccountWebService, AccountWebService>();
builder.Services.AddScoped<ICourseWebService, CourseWebService>();
builder.Services.AddScoped<IReportWebService, ReportWebService>();
builder.Services.AddSingleton<ApplicationContext>(sp =>
    new ApplicationContext(
        sp.GetRequiredService<IClientStorageService>(),
        sp.GetRequiredService<NavigationManager>(),
        sp.GetRequiredService<ICourseWebService>()
    )
);
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBootstrapBlazor();

builder.Services.AddSingleton(Log.Logger);
await builder.Build().RunAsync();