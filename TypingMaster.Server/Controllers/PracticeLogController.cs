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
            [FromQuery] bool sortByNewest = true)
        {
            try
            {
                var result = await practiceLogService.GetPaginatedDrillStatsByPracticeLogIdAsync(
                    id, page, pageSize, sortByNewest);

                return Ok(result);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error retrieving paginated drill stats for account {AccountId}", id);
                return StatusCode(500, new { message = "An error occurred while retrieving drill stats" });
            }
        }
    }
}