using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Mutara.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly ILogger<PlayerController> logger;

        public PlayerController(ILogger<PlayerController> logger)
        {
            this.logger = logger;
        }
 
    }
}