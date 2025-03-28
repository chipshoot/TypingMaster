﻿
using Serilog;

namespace TypingMaster.Core.Utility;

public class ProcessResult(ILogger? logger = null)
{
    public ProcessResultStatus Status { get; set; } = ProcessResultStatus.NotSet;

    public IEnumerable<string> InformationList { get; set; } = [];

    public string ErrorMessage { get; set; } = "";

    public string CallStack { get; set; } = "";

    public void AddError(string errorMessage)
    {
        var message = errorMessage ?? "";
        Status = ProcessResultStatus.Failure;
        ErrorMessage = message;
        logger?.Error(message);
    }

    public void AddException(Exception exception)
    {
        Status = ProcessResultStatus.Failure;
        ErrorMessage = exception.Message;
        CallStack =  exception.StackTrace?? "";
        logger?.Error(exception, exception.Message);
    }

    public void AddSuccess(string message = "")
    {
        Status = ProcessResultStatus.Success;
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        InformationList.ToList().Add(message);
        logger?.Information(message);
    }

    public void AddInformation(string message)
    {
        Status = ProcessResultStatus.NotSet;
        InformationList.ToList().Add(message);
        logger?.Information(message);
    }
}