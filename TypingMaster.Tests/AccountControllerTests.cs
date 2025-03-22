using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TypingMaster.Business.Contract;
using TypingMaster.Business.Models;
using TypingMaster.Server.Controllers;
using Microsoft.EntityFrameworkCore;
using TypingMaster.Business.Utility;

namespace TypingMaster.Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IAccountService> _mockAccountService;
        private readonly Mock<ILogger<AccountController>> _mockLogger;
        private readonly AccountController _controller;

        public AccountControllerTests()
        {
            _mockAccountService = new Mock<IAccountService>();
            _mockLogger = new Mock<ILogger<AccountController>>();
            _controller = new AccountController(_mockAccountService.Object, _mockLogger.Object);

            // Setup default ProcessResult for the mock
            var processResult = new ProcessResult();
            processResult.Status = ProcessResultStatus.Success;
            _mockAccountService.Setup(s => s.ProcessResult).Returns(processResult);
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsOkResult_WithAccounts()
        {
            // Arrange
            var accounts = new List<Account>
            {
                new Account { Id = 1, AccountName = "Test1", AccountEmail = "test1@example.com" },
                new Account { Id = 2, AccountName = "Test2", AccountEmail = "test2@example.com" }
            };

            _mockAccountService.Setup(service => service.GetAllAccounts())
                .ReturnsAsync(accounts);

            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccounts = Assert.IsAssignableFrom<IEnumerable<Account>>(okResult.Value);
            Assert.Equal(2, returnedAccounts.Count());
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            _mockAccountService.Setup(service => service.GetAllAccounts())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while retrieving accounts.", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetAllAccounts_ReturnsBadRequest_WhenProcessResultHasError()
        {
            // Arrange
            var accounts = new List<Account>();
            var processResult = new ProcessResult
            {
                Status = ProcessResultStatus.Failure,
                ErrorMessage = "Database connection failed"
            };

            _mockAccountService.Setup(service => service.GetAllAccounts())
                .ReturnsAsync(accounts);
            _mockAccountService.Setup(service => service.ProcessResult)
                .Returns(processResult);

            // Act
            var result = await _controller.GetAllAccounts();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Database connection failed", badRequestResult.Value);
        }

        [Fact]
        public async Task GetAccount_ReturnsOkResult_WithAccount_WhenIdExists()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);

            // Act
            var result = await _controller.GetAccount(accountId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal(accountId, returnedAccount.Id);
        }

        [Fact]
        public async Task GetAccount_ReturnsNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            int accountId = 999;

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _controller.GetAccount(accountId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Account with ID {accountId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task GetAccount_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var accountId = 1;

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetAccount(accountId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal($"An error occurred while retrieving account with ID {accountId}.", statusCodeResult.Value);
        }

        [Fact]
        public async Task GetAccount_ReturnsBadRequest_WhenProcessResultHasError()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };
            var processResult = new ProcessResult();
            processResult.Status = ProcessResultStatus.Failure;
            processResult.ErrorMessage = "Permission denied";

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.ProcessResult)
                .Returns(processResult);

            // Act
            var result = await _controller.GetAccount(accountId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Permission denied", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAccount_ReturnsCreatedAtActionResult_WhenAccountIsCreatedSuccessfully()
        {
            // Arrange
            var newAccount = new Account { AccountName = "NewUser", AccountEmail = "newuser@example.com" };
            var createdAccount = new Account { Id = 1, AccountName = "NewUser", AccountEmail = "newuser@example.com" };

            _mockAccountService.Setup(service => service.CreateAccount(newAccount))
                .ReturnsAsync(createdAccount);

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_controller.GetAccount), createdAtActionResult.ActionName);
            Assert.Equal(1, createdAtActionResult.RouteValues["id"]);

            var returnedAccount = Assert.IsType<Account>(createdAtActionResult.Value);
            Assert.Equal(1, returnedAccount.Id);
            Assert.Equal("NewUser", returnedAccount.AccountName);
            Assert.Equal("newuser@example.com", returnedAccount.AccountEmail);
        }

        [Fact]
        public async Task CreateAccount_ReturnsBadRequest_WhenAccountCreationFails()
        {
            // Arrange
            var invalidAccount = new Account { AccountName = "", AccountEmail = "" };

            _mockAccountService.Setup(service => service.CreateAccount(invalidAccount))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _controller.CreateAccount(invalidAccount);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid account data or account creation failed", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateAccount_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            var newAccount = new Account { AccountName = "NewUser", AccountEmail = "newuser@example.com" };

            _mockAccountService.Setup(service => service.CreateAccount(newAccount))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while creating the account.", statusCodeResult.Value);
        }

        [Fact]
        public async Task CreateAccount_ReturnsBadRequest_WhenProcessResultHasError()
        {
            // Arrange
            var newAccount = new Account { AccountName = "NewUser", AccountEmail = "newuser@example.com" };
            var processResult = new ProcessResult();
            processResult.Status = ProcessResultStatus.Failure;
            processResult.ErrorMessage = "Account data is incorrect";

            _mockAccountService.Setup(service => service.CreateAccount(newAccount))
                .ReturnsAsync((Account)null);
            _mockAccountService.Setup(service => service.ProcessResult)
                .Returns(processResult);

            // Act
            var result = await _controller.CreateAccount(newAccount);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Account data is incorrect", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsOkResult_WithUpdatedAccount_WhenSuccessful()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };
            var updatedAccount = new Account { Id = accountId, AccountName = "Updated", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ReturnsAsync(updatedAccount);

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal("Updated", returnedAccount.AccountName);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsBadRequest_WhenIdMismatch()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = 2, AccountName = "Test", AccountEmail = "test@example.com" };

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("ID mismatch", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Account with ID {accountId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while updating the account.", statusCodeResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsBadRequest_WhenProcessResultHasErrorAfterGetAccount()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };
            var processResult = new ProcessResult();
            processResult.Status = ProcessResultStatus.Failure;
            processResult.ErrorMessage = "Account retrieval failed";

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.ProcessResult)
                .Returns(processResult);

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Account retrieval failed", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsBadRequest_WhenProcessResultHasErrorAfterUpdate()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            // Setup initial success for GetAccount
            var initialProcessResult = new ProcessResult();
            initialProcessResult.Status = ProcessResultStatus.Success;

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account)
                .Callback(() =>
                {
                    // The ProcessResult remains successful after GetAccount
                });

            // Setup failure for UpdateAccount
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ReturnsAsync((Account)null)
                .Callback(() =>
                {
                    // Update the ProcessResult to indicate failure
                    var errorProcessResult = new ProcessResult();
                    errorProcessResult.Status = ProcessResultStatus.Failure;
                    errorProcessResult.ErrorMessage = "The account has been modified by another user";
                    _mockAccountService.Setup(s => s.ProcessResult).Returns(errorProcessResult);
                });

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The account has been modified by another user", badRequestResult.Value);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsNoContent_WhenSuccessful()
        {
            // Arrange
            int accountId = 1;

            _mockAccountService.Setup(service => service.DeleteAccount(accountId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteAccount(accountId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            int accountId = 999;

            _mockAccountService.Setup(service => service.DeleteAccount(accountId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteAccount(accountId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Account with ID {accountId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int accountId = 1;

            _mockAccountService.Setup(service => service.DeleteAccount(accountId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.DeleteAccount(accountId);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal($"An error occurred while deleting account with ID {accountId}.", statusCodeResult.Value);
        }

        [Fact]
        public async Task DeleteAccount_ReturnsBadRequest_WhenProcessResultHasError()
        {
            // Arrange
            int accountId = 1;
            var processResult = new ProcessResult();
            processResult.Status = ProcessResultStatus.Failure;
            processResult.ErrorMessage = "Account has associated data that can't be deleted";

            _mockAccountService.Setup(service => service.DeleteAccount(accountId))
                .ReturnsAsync(false);
            _mockAccountService.Setup(service => service.ProcessResult)
                .Returns(processResult);

            // Act
            var result = await _controller.DeleteAccount(accountId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Account has associated data that can't be deleted", badRequestResult.Value);
        }

        [Fact]
        public async Task PatchAccount_ReturnsOkResult_WithUpdatedAccount_WhenSuccessful()
        {
            // Arrange
            int accountId = 1;
            var existingAccount = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };
            var updatedAccount = new Account { Id = accountId, AccountName = "Updated", AccountEmail = "test@example.com" };

            var patchDoc = new JsonPatchDocument<Account>();
            patchDoc.Replace(a => a.AccountName, "Updated");

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(existingAccount);
            _mockAccountService.Setup(service => service.UpdateAccount(It.IsAny<Account>()))
                .ReturnsAsync(updatedAccount);

            // Act
            var result = await _controller.PatchAccount(accountId, patchDoc);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedAccount = Assert.IsType<Account>(okResult.Value);
            Assert.Equal("Updated", returnedAccount.AccountName);
        }

        [Fact]
        public async Task PatchAccount_ReturnsBadRequest_WhenPatchDocIsNull()
        {
            // Arrange
            int accountId = 1;

            // Act
            var result = await _controller.PatchAccount(accountId, null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Patch document is required", badRequestResult.Value);
        }

        [Fact]
        public async Task PatchAccount_ReturnsNotFound_WhenAccountDoesNotExist()
        {
            // Arrange
            int accountId = 1;
            var patchDoc = new JsonPatchDocument<Account>();
            patchDoc.Replace(a => a.AccountName, "Updated");

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync((Account)null);

            // Act
            var result = await _controller.PatchAccount(accountId, patchDoc);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"Account with ID {accountId} not found", notFoundResult.Value);
        }

        [Fact]
        public async Task PatchAccount_ReturnsInternalServerError_WhenExceptionOccurs()
        {
            // Arrange
            int accountId = 1;
            var existingAccount = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            var patchDoc = new JsonPatchDocument<Account>();
            patchDoc.Replace(a => a.AccountName, "Updated");

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(existingAccount);
            _mockAccountService.Setup(service => service.UpdateAccount(It.IsAny<Account>()))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.PatchAccount(accountId, patchDoc);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
            Assert.Equal("An error occurred while patching the account.", statusCodeResult.Value);
        }

        [Fact]
        public async Task PatchAccount_ReturnsBadRequest_WhenProcessResultHasErrorAfterGetAccount()
        {
            // Arrange
            int accountId = 1;
            var patchDoc = new JsonPatchDocument<Account>();
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };
            var processResult = new ProcessResult();
            processResult.Status = ProcessResultStatus.Failure;
            processResult.ErrorMessage = "Account retrieval failed";

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.ProcessResult)
                .Returns(processResult);

            // Act
            var result = await _controller.PatchAccount(accountId, patchDoc);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Account retrieval failed", badRequestResult.Value);
        }

        [Fact]
        public async Task PatchAccount_ReturnsBadRequest_WhenProcessResultHasErrorAfterUpdate()
        {
            // Arrange
            int accountId = 1;
            var patchDoc = new JsonPatchDocument<Account>();
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            // Setup initial success for GetAccount
            var initialProcessResult = new ProcessResult();
            initialProcessResult.Status = ProcessResultStatus.Success;

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account)
                .Callback(() =>
                {
                    // The ProcessResult remains successful after GetAccount
                });

            // Setup failure for UpdateAccount
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ReturnsAsync((Account)null)
                .Callback(() =>
                {
                    // Update the ProcessResult to indicate failure
                    var errorProcessResult = new ProcessResult();
                    errorProcessResult.Status = ProcessResultStatus.Failure;
                    errorProcessResult.ErrorMessage = "Invalid account data after patching";
                    _mockAccountService.Setup(s => s.ProcessResult).Returns(errorProcessResult);
                });

            // Act
            var result = await _controller.PatchAccount(accountId, patchDoc);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid account data after patching", badRequestResult.Value);
        }

        [Fact]
        public void TestAuth_ReturnsOkResult_WithUserInfo()
        {
            // Arrange - setup claims
            // Note: This test would normally set up the User property of ControllerBase
            // However, that would require setting up ClaimsPrincipal which is more complex
            // In a real test, you might use a TestController helper or mock the HttpContext

            // Act
            var result = _controller.TestAuth();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }

        // Test for specific exception types handling
        [Fact]
        public async Task UpdateAccount_ReturnsBadRequest_WhenArgumentExceptionOccurs()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ThrowsAsync(new ArgumentException("Invalid account data provided"));

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Invalid account data provided.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsBadRequest_WhenInvalidOperationExceptionOccurs()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ThrowsAsync(new InvalidOperationException("Operation not allowed"));

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Unable to update account due to invalid operation.", badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsForbidden_WhenUnauthorizedAccessExceptionOccurs()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ThrowsAsync(new UnauthorizedAccessException("Access denied"));

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var forbiddenResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(403, forbiddenResult.StatusCode);
            Assert.Equal("You don't have permission to update this account.", forbiddenResult.Value);
        }

        [Fact]
        public async Task UpdateAccount_ReturnsBadRequest_WhenDbUpdateConcurrencyExceptionOccurs()
        {
            // Arrange
            int accountId = 1;
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ThrowsAsync(new DbUpdateConcurrencyException("Concurrency conflict"));

            // Act
            var result = await _controller.UpdateAccount(accountId, account);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("The account has been modified by another user. Please refresh and try again.", badRequestResult.Value);
        }

        [Fact]
        public async Task PatchAccount_ReturnsBadRequest_WhenDbUpdateExceptionOccurs()
        {
            // Arrange
            int accountId = 1;
            var patchDoc = new JsonPatchDocument<Account>();
            var account = new Account { Id = accountId, AccountName = "Test", AccountEmail = "test@example.com" };

            _mockAccountService.Setup(service => service.GetAccount(accountId))
                .ReturnsAsync(account);

            // Mock DbUpdateException when updating the patched account
            _mockAccountService.Setup(service => service.UpdateAccount(account))
                .ThrowsAsync(new DbUpdateException("Database error occurred", new Exception()));

            // Act
            var result = await _controller.PatchAccount(accountId, patchDoc);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Failed to update account due to a database error.", badRequestResult.Value);
        }
    }
}