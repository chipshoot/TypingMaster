using Microsoft.EntityFrameworkCore;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Mapping;
using TypingMaster.DataAccess.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using TypingMaster.Server.Auth;
using Amazon.CognitoIdentityProvider;
using TypingMaster.Business.Config;

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Release";
Console.WriteLine($"Current environment: {environment}");

// Load environment-specific settings first, then base settings
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// Configure specific URLs/ports
// This will override settings in launchSettings.json when explicitly run
var urls = builder.Configuration["Urls"];
if (!string.IsNullOrEmpty(urls))
{
    builder.WebHost.UseUrls(urls);
}

// Configure logging
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAutoMapper(typeof(DomainMapProfile));

// Register Serilog logger
builder.Services.AddSingleton(Log.Logger);

// Register JWT token generator
builder.Services.AddSingleton<JwtTokenGenerator>();

// Configure JWT authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"] ??
                throw new InvalidOperationException("JWT Secret key is not configured"))
        )
    };
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
        policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

// Register repositories
builder.Services.AddScoped<IUserProfileRepository, UserProfileRepository>();
builder.Services.AddScoped<IPracticeLogRepository, PracticeLogRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IDrillStatsRepository, DrillStatsRepository>();
builder.Services.AddScoped<ICourseRepository, CourseRepository>();
builder.Services.AddScoped<ILoginLogRepository, LoginLogRepository>();
builder.Services.AddScoped<ILoginCredentialRepository, LoginCredentialRepository>();

// Configure AWS services
builder.Services.Configure<CognitoSettings>(builder.Configuration.GetSection("CognitoSettings"));
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddAWSService<IAmazonCognitoIdentityProvider>();

// Register business services
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddScoped<IIdpService, MockIdpService>();
}
else
{
    builder.Services.AddScoped<IIdpService, AwsCognitoService>();
}

builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IPracticeLogService, PracticeLogService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ILoginLogService, LoginLogService>();
builder.Services.AddScoped<ILoginCredentialService, LoginCredentialService>();
builder.Services.AddScoped<ICourseService, CourseService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ITypingTrainer, TypingTrainer>();
builder.Services.AddScoped<ITypingMaterialGenerator, TypingMaterialGenerator>();

// Register services
builder.Services.AddScoped<IAuthorizationHandler, IdpAuthorizationHandler>();
builder.Services.AddHttpContextAccessor();

// Configure authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("IdPAuth", policy =>
        policy.Requirements.Add(new IdpAuthorizationRequirement()));
});

// Add HTTP client for external services
builder.Services.AddHttpClient();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TypingMaster API", Version = "v1" });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger(c =>
    {
        c.RouteTemplate = "api/{documentName}/swagger.json";
    });
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/api/v1/swagger.json", "TypingMaster API v1");
        c.RoutePrefix = "swagger";
    });
}

// Add CORS middleware before any other middleware
app.UseCors();

app.UseStaticFiles();
app.UseHttpsRedirection();

// Add authentication middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
