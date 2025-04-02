using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;
using TypingMaster.Core.Utility;

namespace TypingMaster.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "IdPAuth")]
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
                if (accountService.ProcessResult.Status == ProcessResultStatus.Failure)
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
                var account = await accountService.GetAccountById(id);
                if (account == null)
                {
                    return NotFound();
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving account {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the account");
            }
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> GetAccountByEmail(string email)
        {
            try
            {
                var account = await accountService.GetAccountByEmail(email);
                if (account == null)
                {
                    return NotFound();
                }

                return Ok(account);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving account for email {Email}", email);
                return StatusCode(500, "An error occurred while retrieving the account");
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
                    if (accountService.ProcessResult.Status == ProcessResultStatus.Failure)
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
            if (id != account.Id)
            {
                return BadRequest(new AccountResponse
                {
                    Success = false,
                    Message = "Account ID mismatch",
                    StatusCode = System.Net.HttpStatusCode.BadRequest
                });
            }

            var updatedAccount = await accountService.UpdateAccount(account);
            if (updatedAccount == null)
            {
                // Check if there's an error message in the ProcessResult
                if (accountService.ProcessResult.Status == ProcessResultStatus.Failure)
                {
                    return BadRequest(new AccountResponse
                    {
                        Success = false,
                        Message = accountService.ProcessResult.ErrorMessage,
                        StatusCode = System.Net.HttpStatusCode.BadRequest
                    });
                }

                return NotFound(new AccountResponse
                {
                    Success = false,
                    Message = $"Account with ID {id} not found",
                    StatusCode = System.Net.HttpStatusCode.NotFound
                });
            }

            // Create and return AccountResponse with the updated account
            var response = new AccountResponse
            {
                Success = true,
                Message = "Account updated successfully",
                StatusCode = System.Net.HttpStatusCode.OK,
                AccountReturned = updatedAccount
            };

            return Ok(response);
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
                    if (accountService.ProcessResult.Status == ProcessResultStatus.Failure)
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

                var existingAccount = await accountService.GetAccountById(id);

                // Check if there's an error message in the ProcessResult after GetAccountById
                if (accountService.ProcessResult.Status == ProcessResultStatus.Failure)
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
                    if (accountService.ProcessResult.Status == ProcessResultStatus.Failure)
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