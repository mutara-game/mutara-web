using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using mutara_web.Services;

namespace mutara_web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
       

        private readonly ILogger<AuthController> logger;
        private readonly HttpClient client;
        private readonly ConfigClient configClient;

        public AuthController(ILogger<AuthController> logger, ConfigClient configClient)
        {
            this.logger = logger;
            this.configClient = configClient;
            this.client = new HttpClient();

            // TODO get from configuration:
            string user = configClient.GetValue("secrets.yaml", "cognito/clientId").GetAwaiter().GetResult();
            string password = configClient.GetValue("secrets.yaml", "cognito/clientSecret").GetAwaiter().GetResult();
            string userAndPasswordToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(user + ":" + password));
            client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Basic {userAndPasswordToken}");
        }

        [HttpGet]
        public String Get()
        {
            return "Hello, GET";
        }

        [HttpGet("signin")]
        public async Task<String> SignIn(string code) {
            var map = new Dictionary<string, string>();
            map.Add("grant_type", "authorization_code");
            map.Add("code", code);
            map.Add("client_id", "clientid"); // TODO get from config
            map.Add("redirect_uri", "https://localhost:5001/auth/signin"); // TODO get from config
            HttpContent content = new FormUrlEncodedContent(map);
            HttpResponseMessage response = await client.PostAsync("https://mutara-dev.auth.us-west-2.amazoncognito.com/oauth2/token", content);
            logger.LogInformation(response.ToString());
            return await response.Content.ReadAsStringAsync();
            /* response is something like:
{
"id_token": "eybigoldstringJA",
"access_token": "greatbigstring",
"refresh_token": "ealskdfjasdf",
"expires_in": 3600,
"token_type": "Bearer"
}
*/
        }
    }
}
