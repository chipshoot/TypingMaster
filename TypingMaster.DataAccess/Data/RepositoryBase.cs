using TypingMaster.DataAccess.Utility;
using ILogger = Serilog.ILogger;

namespace TypingMaster.DataAccess.Data;


public abstract class RepositoryBase(ILogger logger)
{
    public ProcessResult ProcessResult { get; set; } = new(logger);
}