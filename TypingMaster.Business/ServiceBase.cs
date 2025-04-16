using Serilog;
using TypingMaster.Core.Utility;

namespace TypingMaster.Business;

public abstract class ServiceBase(ILogger logger)
{
    public ProcessResult ProcessResult { get; set; } = new(logger);
}