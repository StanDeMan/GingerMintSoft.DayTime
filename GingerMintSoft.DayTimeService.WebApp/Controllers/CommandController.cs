using Microsoft.AspNetCore.Mvc;
using GingerMintSoft.DayTimeService.WebApp.Models;

namespace GingerMintSoft.DayTimeService.WebApp.Controllers
{
    [ApiController]
    [Route("Command")]
    public class VersionsController(ILogger<VersionsController> logger) : Controller
    {
        private readonly ILogger<VersionsController> _logger = logger;

        [HttpPost]
        [Route("Send")]
        public async Task<ActionResult> Post([FromBody] Execute execute)
        {
            try
            {
                var status = await Command.Bash.ExecuteAsync(execute.Command!);

                return new CreatedResult($"/Command/Send", status);
            }
            catch (Exception e)
            {
                var error = $"Cannot send command: {e}";

                _logger.LogError(error);
                return Conflict(error);
            }
        }
    }
}
