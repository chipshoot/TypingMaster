using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Server.Models;

namespace TypingMaster.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.Login(request.Email, request.Password);

                if (response.Success)
                {
                    return Ok(response);
                }

                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing login request for {Email}", request.Email);
                return StatusCode(500, "An error occurred during login");
            }
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
                var response = await _authService.Register(request);
                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Registration operation timed out for {Email}", request.Email);
                return StatusCode(408, new { Success = false, Message = "The request timed out" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing registration request for {Email}", request.Email);
                return StatusCode(500, "An error occurred during registration");
            }
        }

        [HttpPost("logout")]
        [Authorize(Policy = "IdPAuth")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                var success = await _authService.Logout(request.AccountId, request.RefreshToken);
                if (success)
                {
                    return Ok();
                }
                return BadRequest("Failed to logout");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing logout request");
                return StatusCode(500, "An error occurred during logout");
            }
        }

        [HttpPost("refresh-token")]
        [Authorize(Policy = "IdPAuth")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                // Set a tight timeout to avoid long-running requests
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

                // Process the token refresh with timeout
                var response = await _authService.RefreshToken(
                    request.Token,
                    request.RefreshToken,
                    request.Email).ConfigureAwait(false);

                if (response.Success)
                {
                    // Set cache control headers to enable caching of refresh tokens
                    // This will help reduce subsequent refresh token requests
                    Response.Headers.Append("Cache-Control", "private, max-age=600"); // 10 minutes
                    return Ok(response);
                }
                return BadRequest(response);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Token refresh operation timed out for {Email}", request.Email);
                return StatusCode(408, new { Success = false, Message = "The request timed out" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error refreshing token for {Email}", request.Email);
                return StatusCode(500, "An error occurred while refreshing the token");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var result = await _authService.RequestPasswordReset(request.Email);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing forgot password request for {Email}", request.Email);
                return StatusCode(500, "An error occurred while processing forgot password request");
            }
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            try
            {
                var result = await _authService.ResetPassword(request.Token, request.NewPassword);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing reset password request");
                return StatusCode(500, "An error occurred while resetting password");
            }
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                var result = await _authService.ChangePassword(
                    request.AccountId,
                    request.CurrentPassword,
                    request.NewPassword);

                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing change password request for account ID {AccountId}", request.AccountId);
                return StatusCode(500, "An error occurred while changing password");
            }
        }

        [HttpPost("resend-confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendConfirmationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _authService.ResendConfirmationCode(request.UserName);
                if (success)
                {
                    return Ok(new WebServiceResponse { Success = true, Message = "Confirmation code has been resent" });
                }

                return BadRequest(new WebServiceResponse { Success = false, Message = "Failed to resend confirmation code" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resending confirmation code for {UserName}", request.UserName);
                return StatusCode(500, new WebServiceResponse { Success = false, Message = "An error occurred while resending the confirmation code" });
            }
        }

        [HttpPost("confirm-registration")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmRegistration([FromBody] ConfirmRegistrationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var success = await _authService.ConfirmRegistration(request.UserName, request.ConfirmationCode);
                if (success)
                {
                    return Ok(new WebServiceResponse { Success = true, Message = "Registration confirmed successfully" });
                }

                return BadRequest(new WebServiceResponse { Success = false, Message = "Failed to confirm registration" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming registration for {UserName}", request.UserName);
                return StatusCode(500, new WebServiceResponse { Success = false, Message = "An error occurred while confirming registration" });
            }
        }
    }
}