using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TypingMaster.Business.Contract;
using TypingMaster.Core.Models;

namespace TypingMaster.Server.Controllers
{
    [Route("api/practicelogs")]
    [ApiController]
    [Authorize(Policy = "IdPAuth")]
    public class PracticeLogController(IPracticeLogService practiceLogService, ILogger<PracticeLogController> logger)
        : ControllerBase
    {
        [HttpGet("{id}/drill-stats")]
        public async Task<ActionResult<PagedResult<DrillStats>>> GetPaginatedDrillStats(
            int id,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool sortByNewest = true,
            [FromQuery] TrainingType? type = null)
        {
            try
            {
                var result = await practiceLogService.GetPaginatedDrillStatsByPracticeLogId(
                    id, page, pageSize, sortByNewest, type);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving paginated drill stats for account {AccountId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving drill stats" });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PracticeLog>> UpdatePracticeLog(int id, [FromBody] PracticeLog practiceLog)
        {
            try
            {
                var result = await practiceLogService.UpdatePracticeLog(practiceLog);
                if (result == null)
                {
                    return BadRequest(new
                        { message = $"Fail to add drill stat: {practiceLogService.ProcessResult.ErrorMessage}" });
                }

                return CreatedAtAction(nameof(AddDrillStats), new { id = id }, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error update practice log {PracticeLogId}", id);
                return StatusCode(500, new { message = "An error occurred while adding drill stats" });
            }
        }

        [HttpPost("{id}/drill-stats")]
        public async Task<ActionResult<DrillStats>> AddDrillStats(int id, [FromBody] DrillStats drillStats)
        {
            try
            {
                var result = await practiceLogService.AddDrillStat(id, drillStats);
                if (result == null)
                {
                    return BadRequest(new
                        { message = $"Fail to add drill stat: {practiceLogService.ProcessResult.ErrorMessage}" });
                }

                return CreatedAtAction(nameof(AddDrillStats), new { id = id }, result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding drill stat to practice log {PracticeLogId}", id);
                return StatusCode(500, new { message = "An error occurred while adding drill stats" });
            }
        }
    }
}