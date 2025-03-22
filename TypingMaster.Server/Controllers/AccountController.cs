using Microsoft.AspNetCore.Mvc;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

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

                // Check if there's an error message in the ProcessResult
                if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                {
                    return BadRequest(accountService.ProcessResult.ErrorMessage);
                }

                return Ok(accounts);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving all accounts");
                string errorMessage = "An error occurred while retrieving accounts.";

                if (ex is InvalidOperationException)
                {
                    errorMessage = "Unable to retrieve accounts due to invalid operation.";
                    return BadRequest(errorMessage);
                }
                else if (ex is UnauthorizedAccessException)
                {
                    errorMessage = "You don't have permission to access accounts.";
                    return StatusCode(403, errorMessage);
                }

                return StatusCode(500, errorMessage);
            }
        }

        [HttpGet("test-auth")]
        public IActionResult TestAuth()
        {
            // Get the current user's identity from claims
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userName = User.FindFirst(System.Security.Claims.ClaimTypes.Name)?.Value;
            var email = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

            return Ok(new
            {
                Message = "Authentication successful!",
                UserId = userId,
                UserName = userName,
                Email = email,
                Claims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAccount(int id)
        {
            try
            {
                var account = await accountService.GetAccount(id);

                // Check if there's an error message in the ProcessResult
                if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                {
                    return BadRequest(accountService.ProcessResult.ErrorMessage);
                }

                if (account == null)
                    return NotFound($"Account with ID {id} not found");

                return Ok(account);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving account with ID {Id}", id);
                string errorMessage = $"An error occurred while retrieving account with ID {id}.";

                if (ex is InvalidOperationException)
                {
                    errorMessage = "Unable to retrieve account due to invalid operation.";
                    return BadRequest(errorMessage);
                }
                else if (ex is UnauthorizedAccessException)
                {
                    errorMessage = "You don't have permission to access this account.";
                    return StatusCode(403, errorMessage);
                }

                return StatusCode(500, errorMessage);
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateAccount([FromBody] Account account)
        {
            try
            {
                var createdAccount = await accountService.CreateAccount(account);
                if (createdAccount == null)
                {
                    // Check if there's an error message in the ProcessResult
                    if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                    {
                        return BadRequest(accountService.ProcessResult.ErrorMessage);
                    }
                    return BadRequest("Invalid account data or account creation failed");
                }

                return CreatedAtAction(nameof(GetAccount), new { id = createdAccount.Id }, createdAccount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error creating account");
                string errorMessage = "An error occurred while creating the account.";

                if (ex is ArgumentException)
                {
                    errorMessage = "Invalid account data provided.";
                    return BadRequest(errorMessage);
                }
                else if (ex is InvalidOperationException)
                {
                    errorMessage = "Unable to create account due to invalid operation.";
                    return BadRequest(errorMessage);
                }
                else if (ex is UnauthorizedAccessException)
                {
                    errorMessage = "You don't have permission to create an account.";
                    return StatusCode(403, errorMessage);
                }
                else if (ex is DbUpdateException)
                {
                    errorMessage = "Failed to create account due to a database error. The email might already be in use.";
                    return BadRequest(errorMessage);
                }

                return StatusCode(500, errorMessage);
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

                // Check if there's an error message in the ProcessResult after GetAccount
                if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                {
                    return BadRequest(accountService.ProcessResult.ErrorMessage);
                }

                if (existingAccount == null)
                    return NotFound($"Account with ID {id} not found");

                var updatedAccount = await accountService.UpdateAccount(account);
                if (updatedAccount == null)
                {
                    // Check if there's an error message in the ProcessResult
                    if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                    {
                        return BadRequest(accountService.ProcessResult.ErrorMessage);
                    }
                    return BadRequest("Failed to update account, but no specific error was provided");
                }

                return Ok(updatedAccount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error updating account with ID {Id}", id);
                string errorMessage = "An error occurred while updating the account.";

                // Adding more specific error messages based on exception type
                if (ex is ArgumentException)
                {
                    errorMessage = "Invalid account data provided.";
                    return BadRequest(errorMessage);
                }
                else if (ex is InvalidOperationException)
                {
                    errorMessage = "Unable to update account due to invalid operation.";
                    return BadRequest(errorMessage);
                }
                else if (ex is UnauthorizedAccessException)
                {
                    errorMessage = "You don't have permission to update this account.";
                    return StatusCode(403, errorMessage);
                }
                else if (ex is TimeoutException)
                {
                    errorMessage = "The request timed out while updating the account. Please try again.";
                }
                else if (ex is DbUpdateConcurrencyException)
                {
                    errorMessage = "The account has been modified by another user. Please refresh and try again.";
                    return BadRequest(errorMessage);
                }
                else if (ex is DbUpdateException)
                {
                    errorMessage = "Failed to update account due to a database error.";
                    return BadRequest(errorMessage);
                }

                return StatusCode(500, errorMessage);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAccount(int id)
        {
            try
            {
                var result = await accountService.DeleteAccount(id);
                if (!result)
                {
                    // Check if there's an error message in the ProcessResult
                    if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                    {
                        return BadRequest(accountService.ProcessResult.ErrorMessage);
                    }
                    return NotFound($"Account with ID {id} not found");
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error deleting account with ID {Id}", id);
                string errorMessage = $"An error occurred while deleting account with ID {id}.";

                if (ex is ArgumentException)
                {
                    errorMessage = "Invalid account ID provided.";
                    return BadRequest(errorMessage);
                }
                else if (ex is InvalidOperationException)
                {
                    errorMessage = "Unable to delete account due to invalid operation.";
                    return BadRequest(errorMessage);
                }
                else if (ex is UnauthorizedAccessException)
                {
                    errorMessage = "You don't have permission to delete this account.";
                    return StatusCode(403, errorMessage);
                }
                else if (ex is DbUpdateException)
                {
                    errorMessage = "Failed to delete account due to database constraints. The account may have associated records.";
                    return BadRequest(errorMessage);
                }

                return StatusCode(500, errorMessage);
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

                // Check if there's an error message in the ProcessResult after GetAccount
                if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                {
                    return BadRequest(accountService.ProcessResult.ErrorMessage);
                }

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
                if (updatedAccount == null)
                {
                    // Check if there's an error message in the ProcessResult
                    if (accountService.ProcessResult.Status == TypingMaster.Business.Utility.ProcessResultStatus.Failure)
                    {
                        return BadRequest(accountService.ProcessResult.ErrorMessage);
                    }
                    return BadRequest("Failed to update account, but no specific error was provided");
                }

                return Ok(updatedAccount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error patching account with ID {Id}", id);
                string errorMessage = "An error occurred while patching the account.";

                // Adding more specific error messages based on exception type
                if (ex is ArgumentException)
                {
                    errorMessage = "Invalid account data provided.";
                    return BadRequest(errorMessage);
                }
                else if (ex is InvalidOperationException)
                {
                    errorMessage = "Unable to update account due to invalid operation.";
                    return BadRequest(errorMessage);
                }
                else if (ex is UnauthorizedAccessException)
                {
                    errorMessage = "You don't have permission to update this account.";
                    return StatusCode(403, errorMessage);
                }
                else if (ex is DbUpdateConcurrencyException)
                {
                    errorMessage = "The account has been modified by another user. Please refresh and try again.";
                    return BadRequest(errorMessage);
                }
                else if (ex is DbUpdateException)
                {
                    errorMessage = "Failed to update account due to a database error.";
                    return BadRequest(errorMessage);
                }
                else if (ex is JsonPatchException)
                {
                    errorMessage = "Invalid patch document format.";
                    return BadRequest(errorMessage);
                }

                return StatusCode(500, errorMessage);
            }
        }
    }
}