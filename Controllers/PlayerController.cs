using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mutara.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> logger;

        public PlayerController(ILogger<PlayerController> logger)
        {
            this.logger = logger;
        }

        [HttpGet("details/{playerId}")]
        public string Details(Guid playerId)
        {
            return $"hello player {playerId}";
        }
    }
}