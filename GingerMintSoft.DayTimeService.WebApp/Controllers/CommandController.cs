﻿using DayTimeService.Execute;
using Microsoft.AspNetCore.Mvc;
using GingerMintSoft.DayTimeService.WebApp.Models;
using Task = System.Threading.Tasks.Task;

namespace GingerMintSoft.DayTimeService.WebApp.Controllers
{
    [ApiController]
    [Route("Command")]
    public class VersionsController(ILogger<VersionsController> logger) : Controller
    {
        private readonly ILogger<VersionsController> _logger = logger;

        [HttpPost]
        [Route("Send")]
        public async Task<ActionResult> Post([FromBody] Instruction instruction)
        {
            try
            {
                var status = Command.Execute(instruction.Command!);
                await Task.Delay(0);

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
