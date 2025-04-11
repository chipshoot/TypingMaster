using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using TypingMaster.Business;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Mapping;
using TypingMaster.DataAccess.Data;
using TypingMaster.Server.Auth;

namespace TypingMaster.Server;

/// <summary>
/// AWS Lambda startup configuration that mirrors the Program.cs minimal API setup
/// </summary>
public class LambdaStartup
{
    public void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        // Configure logging
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        // Add services to the container
        services.AddControllers();
        services.AddDbContext<ApplicationDbContext>(opt =>
            opt.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddAutoMapper(typeof(DomainMapProfile));

        // Register Serilog logger
        services.AddSingleton(Log.Logger);

        // Register JWT token generator
        services.AddSingleton<JwtTokenGenerator>();

        // Configure JWT authentication
        services.AddAuthentication(options =>
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
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["JwtSettings:Secret"] ??
                                               throw new InvalidOperationException("JWT Secret key is not configured"))
                    )
                };
            });

        // Configure CORS
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                var allowedOrigins = configuration.GetSection("CorsSettings:AllowedOrigins").Get<string[]>();
                policy.WithOrigins(allowedOrigins ?? Array.Empty<string>())
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
            });
        });

        // Register repositories
        services.AddScoped<IUserProfileRepository, UserProfileRepository>();
        services.AddScoped<IPracticeLogRepository, PracticeLogRepository>();
        services.AddScoped<IAccountRepository, AccountRepository>();
        services.AddScoped<IDrillStatsRepository, DrillStatsRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ILoginLogRepository, LoginLogRepository>();
        services.AddScoped<ILoginCredentialRepository, LoginCredentialRepository>();

        // Register business services
        services.AddScoped<MockIdpService>();
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IPracticeLogService, PracticeLogService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ILoginLogService, LoginLogService>();
        services.AddScoped<ILoginCredentialService, LoginCredentialService>();
        services.AddScoped<ICourseService, CourseService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ITypingTrainer, TypingTrainer>();
        services.AddScoped<ITypingMaterialGenerator, TypingMaterialGenerator>();

        // Register services
        services.AddScoped<MockIdpService>();
        services.AddScoped<IAuthorizationHandler, IdpAuthorizationHandler>();
        services.AddHttpContextAccessor();

        // Configure authorization
        services.AddAuthorization(options =>
        {
            options.AddPolicy("IdPAuth", policy =>
                policy.Requirements.Add(new IdpAuthorizationRequirement()));
        });

        // Add HTTP client for external services
        services.AddHttpClient();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        services.AddOpenApi();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new() { Title = "TypingMaster API", Version = "v1" });
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Configure the HTTP request pipeline.
        if (env.IsDevelopment())
        {
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

        // Add routing middleware
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}