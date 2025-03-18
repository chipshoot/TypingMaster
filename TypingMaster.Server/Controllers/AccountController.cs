using Microsoft.AspNetCore.Mvc;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using Microsoft.AspNetCore.JsonPatch;

namespace TypingMaster.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController(IAccountService accountService, ILogger<AccountController> logger)
        : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAccounts()
        {
            try
            {
                var accounts = await accountService.GetAllAccounts();
                return Ok(accounts);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all accounts");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            try
            {
                var account = await accountService.GetAccount(id);
                if (account == null)
                    return NotFound($"Account with ID {id} not found");

                return Ok(account);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving account with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAccount(int id, [FromBody] Account account)
        {
            try
            {
                if (id != account.Id)
                    return BadRequest("ID mismatch");

                var existingAccount = await accountService.GetAccount(id);
                if (existingAccount == null)
                    return NotFound($"Account with ID {id} not found");

                var updatedAccount = await accountService.UpdateAccount(account);
                return Ok(updatedAccount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating account with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var result = await accountService.DeleteAccount(id);
                if (!result)
                    return NotFound($"Account with ID {id} not found");

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting account with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAccount(int id, [FromBody] JsonPatchDocument<Account> patchDoc)
        {
            try
            {
                if (patchDoc == null)
                    return BadRequest("Patch document is required");

                var existingAccount = await accountService.GetAccount(id);
                if (existingAccount == null)
                    return NotFound($"Account with ID {id} not found");

                // Apply the patch document to the existing account
                patchDoc.ApplyTo(existingAccount);

                // Validate the patched model
                if (!TryValidateModel(existingAccount))
                {
                    return BadRequest(ModelState);
                }

                // Update the account with the patched changes
                var updatedAccount = await accountService.UpdateAccount(existingAccount);
                return Ok(updatedAccount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error patching account with ID {Id}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        //[HttpGet("profile/{id}")]
        //public async Task<IActionResult> GetProfile(int id)
        //{
        //    try
        //    {
        //        var userProfile = await accountService.GetUserProfile(id);
        //        if (userProfile == null)
        //            return NotFound($"Profile for account ID {id} not found");

        //        return Ok(userProfile);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Error retrieving profile for account ID {Id}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //[HttpPut("profile/{id}")]
        //public async Task<IActionResult> UpdateProfile(int id, [FromBody] UserProfile profileDto)
        //{
        //    try
        //    {
        //        var updatedProfile = await accountService.UpdateUserProfile(id, profileDto);
        //        if (updatedProfile == null)
        //            return NotFound($"Profile for account ID {id} not found");

        //        return Ok(updatedProfile);
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogError(ex, "Error updating profile for account ID {Id}", id);
        //        return StatusCode(500, "Internal server error");
        //    }
        //}

        //todo: add register method, login method, and other account related methods
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] Account loginDto)
        {
            // Implement account login logic here
            // For example, validate account credentials and generate a token

            return Ok(new { Token = "GeneratedToken" });
        }
    }
}