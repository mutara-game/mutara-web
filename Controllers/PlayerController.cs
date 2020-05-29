using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mutara.Web.Api;
using Mutara.Web.Api.Player;

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
        public ApiOkResponse<PlayerDetailsResponse> Details(Guid playerId)
        {
            return new ApiOkResponse<PlayerDetailsResponse>(
                new PlayerDetailsResponse
                {
                    PlayerId = playerId,
                    Magic = "Foooooooooooooo"
                }
            );
        }
    }

}