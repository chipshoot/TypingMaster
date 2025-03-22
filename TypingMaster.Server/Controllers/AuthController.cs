using Microsoft.AspNetCore.Mvc;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
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
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.LoginAsync(request.Email, request.Password);

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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var response = await _authService.RegisterAsync(request);

                if (response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing registration request for {Email}", request.Email);
                return StatusCode(500, "An error occurred during registration");
            }
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequest request)
        {
            try
            {
                var result = await _authService.LogoutAsync(request.AccountId, request.RefreshToken);
                return Ok(new { Success = result });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing logout request for account ID {AccountId}", request.AccountId);
                return StatusCode(500, "An error occurred during logout");
            }
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var response = await _authService.RefreshTokenAsync(request.Token, request.RefreshToken);

                if (response.Success)
                {
                    return Ok(response);
                }

                return Unauthorized(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing refresh token request");
                return StatusCode(500, "An error occurred while refreshing token");
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            try
            {
                var result = await _authService.RequestPasswordResetAsync(request.Email);
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
                var result = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
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
                var result = await _authService.ChangePasswordAsync(
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
    }
}