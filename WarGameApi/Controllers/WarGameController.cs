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
        public IActionResult Get()
        {
            Game game = new Game();

            game.StartGame("playe1", "player2");

            return Ok("OK");
        }

    }
}
