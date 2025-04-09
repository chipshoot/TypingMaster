
using TypingMaster.Core.Models;

namespace TypingMaster.Client.Services
{
    public class AccountClientService(
        IAccountWebService accountService,
        ICourseWebService courseService,
        Serilog.ILogger logger) : IAccountClientService
    {
        public async Task AddCourseToAccountAsync(ApplicationContext applicationState)
        {
            try
            {
                if (applicationState?.CurrentAccount == null)
                {
                    logger.Error("Null Account Found");
                    return;
                }

                // check if course already exists in account
                var existingCourse = await courseService.GetCourse(applicationState.CurrentCourse.Id);
                if (existingCourse != null)
                {
                    logger.Error($"The course: {applicationState.CurrentCourse.Id} already exists in database");

                }

                // save new added course to table
                var createdCourse = await courseService.CreateCourse(applicationState.CurrentCourse);
                if (createdCourse == null)
                {
                    logger.Error($"Failed to create course. Details: {applicationState?.CurrentCourse?.Id}");
                    return;
                }

                var updateResult = await accountService.UpdateAccountAsync(applicationState.CurrentAccount);
                if (updateResult.Success)
                {
                    applicationState.CurrentAccount = updateResult.AccountReturned;
                    logger.Information("Account updated successfully");
                }
                else
                {
                    logger.Error($"Failed to update account. Details: {applicationState.CurrentAccount.Id}");
                }

                logger.Information("Account updated successfully");
            }
            catch (Exception e)
            {
                logger.Error(e.Message);
                throw;
            }
        }

        public async Task<Account?> GetGuestAccount()
        {
            return await accountService.GetGuestAccount();
        }
    }
}
