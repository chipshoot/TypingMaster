using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TypingMaster.Server.Dto;

namespace TypingMaster.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ILogger<AccountController> _logger;

        public AccountController(ILogger<AccountController> logger)
        {
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] AccountRegistrationDto registrationDto)
        {
            // Implement account registration logic here
            // For example, save the account to the database

            return Ok(new { Message = "Account registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] AccountLoginDto loginDto)
        {
            // Implement account login logic here
            // For example, validate account credentials and generate a token

            return Ok(new { Token = "GeneratedToken" });
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            // Implement logic to retrieve account profile information
            // For example, get the account information from the database

            var accountProfile = new UserProfileDto
            {
                Id = 1,
                AccountNumberId = 1,
                Email = "user@example.com",
                FirstName = "Example User",
                LastName = "Example User"
            };

            return Ok(accountProfile);
        }

        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto profileDto)
        {
            // Implement logic to update account profile information
            // For example, update the account information in the database

            return Ok(new { Message = "Profile updated successfully" });
        }
    }
}
