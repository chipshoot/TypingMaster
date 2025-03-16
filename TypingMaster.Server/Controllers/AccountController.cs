//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using TypingMaster.Business.Models;
//using TypingMaster.Server.Data;
//using TypingMaster.Server.Dto;

//namespace TypingMaster.Server.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class AccountController(IAccountRepository accountRepository, ILogger<AccountController> logger)
//        : ControllerBase
//    {
//        private readonly IAccountRepository _accountRepository = accountRepository;
//        private readonly ILogger<AccountController> _logger = logger;

//        [HttpGet]
//        public async Task<IActionResult> GetAllAccounts()
//        {
//            try
//            {
//                var accounts = await _accountRepository.GetAllAccountsAsync();
//                return Ok(accounts);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving all accounts");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpGet("{id}")]
//        public async Task<IActionResult> GetAccount(int id)
//        {
//            try
//            {
//                var account = await _accountRepository.GetAccountByIdAsync(id);
//                if (account == null)
//                    return NotFound($"Account with ID {id} not found");

//                return Ok(account);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error retrieving account with ID {Id}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpPost("register")]
//        public async Task<IActionResult> Register([FromBody] AccountRegistrationDto registrationDto)
//        {
//            try
//            {
//                // Check if email already exists
//                var existingAccount = await _accountRepository.GetAccountByEmailAsync(registrationDto.Email);
//                if (existingAccount != null)
//                    return BadRequest("An account with this email already exists");

//                // Create new account from DTO
//                var account = new Account
//                {
//                    AccountName = registrationDto.AccountNumber,
//                    AccountEmail = registrationDto.Email,
//                    User = new UserProfileDao
//                    {
//                        FirstName = registrationDto.UserProfile.FirstName,
//                        LastName = registrationDto.UserProfile.LastName
//                    },
//                    History = new PracticeLogDao(),
//                    CourseId = Guid.NewGuid(),
//                    TestCourseId = Guid.NewGuid(),
//                    GameCourseId = Guid.NewGuid()
//                };

//                // Save to database
//                var createdAccount = await _accountRepository.CreateAccountAsync(account);
//                return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error registering new account");
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account account)
//        {
//            try
//            {
//                if (id != account.Id)
//                    return BadRequest("ID mismatch");

//                var existingAccount = await _accountRepository.GetAccountByIdAsync(id);
//                if (existingAccount == null)
//                    return NotFound($"Account with ID {id} not found");

//                var updatedAccount = await _accountRepository.UpdateAccountAsync(account);
//                return Ok(updatedAccount);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error updating account with ID {Id}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }

//        [HttpDelete("{id}")]
//        public async Task<IActionResult> DeleteAccount(int id)
//        {
//            try
//            {
//                var result = await _accountRepository.DeleteAccountAsync(id);
//                if (!result)
//                    return NotFound($"Account with ID {id} not found");

//                return NoContent();
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Error deleting account with ID {Id}", id);
//                return StatusCode(500, "Internal server error");
//            }
//        }
//        [HttpPost("login")]
//        public async Task<IActionResult> Login([FromBody] Account loginDto)
//        {
//            // Implement account login logic here
//            // For example, validate account credentials and generate a token

//            return Ok(new { Token = "GeneratedToken" });
//        }

//        [HttpGet("profile")]
//        public async Task<IActionResult> GetProfile()
//        {
//            // Implement logic to retrieve account profile information
//            // For example, get the account information from the database

//            var accountProfile = new UserProfile();
//            //{
//            //    Id = 1,
//            //    AccountNumberId = 1,
//            //    Email = "user@example.com",
//            //    FirstName = "Example User",
//            //    LastName = "Example User"
//            //};

//            return Ok(accountProfile);
//        }

//        [HttpPut("profile")]
//        public async Task<IActionResult> UpdateProfile([FromBody] UserProfile profileDto)
//        {
//            // Implement logic to update account profile information
//            // For example, update the account information in the database

//            return Ok(new { Message = "Profile updated successfully" });
//        }
//    }
//}