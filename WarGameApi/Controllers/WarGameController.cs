using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WarGameApi.Controllers
{
    [Route("api/startgame")]
    [ApiController]
    public class WarGameController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get([FromQuery] string p1, [FromQuery] string p2)
        {
            if (string.IsNullOrWhiteSpace(p1) || string.IsNullOrWhiteSpace(p2))
                return BadRequest("You must provide name of Players.");

            Game game = new Game();

            GameResult res = game.StartGame(p1, p2);

            return Ok(res);
        }

    }
}
