using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Serilog;
using TypingMaster.Business.Contract;
using TypingMaster.Business;
using TypingMaster.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.BrowserConsole()
    .CreateLogger();

builder.Logging.AddSerilog(Log.Logger);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton<TypingTrainerContext>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ITypingTrainer, TypingTrainer>();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddBootstrapBlazor();

builder.Services.AddSingleton(Log.Logger);
await builder.Build().RunAsync();
