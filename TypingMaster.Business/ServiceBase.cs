using Serilog;
using TypingMaster.Business.Utility;

namespace TypingMaster.Business;

public abstract class ServiceBase(ILogger logger)
{
    protected const string InvalidAccountData = "Account data is incorrect.";
    protected const string NoAccount = "Account cannot be found.";
    protected const string NoKeyPressCount = "No key press count.";
    protected const string NoCorrectKeyPressCount = "No correct key press count.";

    public ProcessResult ProcessResult { get; set; } = new(logger);
}