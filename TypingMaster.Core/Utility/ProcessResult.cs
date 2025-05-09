
using Serilog;

namespace TypingMaster.Core.Utility;

public class ProcessResult(ILogger? logger = null)
{
    private readonly List<string> _informationList = [];

    public ProcessResultStatus Status { get; set; } = ProcessResultStatus.NotSet;

    public IEnumerable<string> InformationList => _informationList;

    public string ErrorMessage { get; set; } = "";

    public string CallStack { get; set; } = "";

    public bool HasErrors => Status == ProcessResultStatus.Failure || !string.IsNullOrEmpty(ErrorMessage);

    public void AddError(string errorMessage)
    {
        Status = ProcessResultStatus.Failure;
        ErrorMessage = errorMessage;
        logger?.Error(errorMessage);
    }

    public void AddException(Exception exception)
    {
        Status = ProcessResultStatus.Failure;
        ErrorMessage = exception.Message;
        CallStack = exception.StackTrace ?? "";
        logger?.Error(exception, exception.Message);
    }

    public void AddSuccess()
    {
        Status = ProcessResultStatus.Success;
    }

    public void AddInformation(string message)
    {
        Status = ProcessResultStatus.NotSet;
        _informationList.Add(message);
        logger?.Information(message);
    }

    public void PropagateResult(ProcessResult? innerResult)
    {
        if (innerResult == null)
        {
            return;
        }

        Status = innerResult.Status;
        _informationList.AddRange(innerResult.InformationList);
        ErrorMessage = string.IsNullOrEmpty(ErrorMessage)
            ? innerResult.ErrorMessage
            : $"{ErrorMessage}:{innerResult.ErrorMessage}";

        if (InformationList.Any())
        {
            logger?.Information("{@InformationList}", InformationList);
        }

        if (!string.IsNullOrEmpty(ErrorMessage))
        {
            logger?.Error(ErrorMessage);
        }
    }

    public void ClearInformation()
    {
        _informationList.Clear();
    }
}