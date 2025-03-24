using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace TypingMaster.Server.Auth;

public class IdpAuthorizationHandler : AuthorizationHandler<IdpAuthorizationRequirement>
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public IdpAuthorizationHandler(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        IdpAuthorizationRequirement requirement)
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            return Task.CompletedTask;
        }

        var authHeader = httpContext.Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return Task.CompletedTask;
        }

        var token = authHeader.Substring("Bearer ".Length);

        // For development, we'll accept any token
        // In production, this would validate the token against the IDP
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "Development User"),
            new Claim(ClaimTypes.Email, "dev@example.com")
        };

        var identity = new ClaimsIdentity(claims, "IdP");
        httpContext.User = new ClaimsPrincipal(identity);
        context.Succeed(requirement);

        return Task.CompletedTask;
    }
}

public class IdpAuthorizationRequirement : IAuthorizationRequirement
{
}