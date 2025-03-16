using TypingMaster.Business.Utility;
using ILogger = Serilog.ILogger;

namespace TypingMaster.Server.Data;

public abstract class RepositoryBase(ILogger logger)
{
    public ProcessResult ProcessResult { get; set; } = new(logger);
}