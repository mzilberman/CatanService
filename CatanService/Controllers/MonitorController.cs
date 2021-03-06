﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CatanSharedModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CatanService.Controllers
{

    [Route("api/catan/monitor")]
    [ApiController]
    public class MonitorController : ControllerBase
    {
        [HttpGet("{gameName}/{playerName}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> MonitorResources(string gameName, string playerName)
        {
            var game = TSGlobal.GetGame(gameName);
            if (game == null)
            {
                return NotFound(new CatanResult() { Description = $"Game '{gameName}' does not exist", Request = this.Request.Path });
            }
            //if (!game.Started)
            //{
            //    return BadRequest(new CatanResult() { Request = this.Request.Path, Description = $"{playerName} in '{gameName}' tried to Monitor a game that hasn't started yet." });
            //}

            //
            //  need this to get the TaskCompletionSource
            var clientState = game.GetPlayer(playerName);
            
            if (clientState == null)
            {

                return NotFound(new CatanResult() { Request = this.Request.Path, Description = $"{playerName} in game '{gameName}' not found" });

            }
            var list = await clientState.TSWaitForLog();
            if (list.Count == 0)
            {
                return BadRequest(new CatanResult() { Request = this.Request.Path, Description = $"Why did {playerName} release with no log entries?" });
            }
            return Ok(list);

        }

    }
}