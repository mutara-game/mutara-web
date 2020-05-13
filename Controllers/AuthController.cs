using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace mutara_web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
       

        private readonly ILogger<AuthController> logger;

        public AuthController(ILogger<AuthController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public String Get()
        {
            return "Hello, GET";
        }

        [HttpGet("signin")]
        public String SignIn(string code) {

            System.Net.HttpClient client =new System.Net.HttpClient();



            return "Hello Signin your code is " + code;
            /*
            The custom application that’s hosted at the redirect URL can then extract the authorization code from the query parameters and exchange it for user pool tokens. 
            The exchange occurs by submitting a POST request to https://AUTH_DOMAIN/oauth2/token with the following application/x-www-form-urlencoded parameters:
grant_type – Set to “authorization_code” for this grant.
code – The authorization code that’s vended to the user.
client_id – Same as from the request in step 1.
redirect_uri – Same as from the request in step 1.
code_verifier (optional, is required if a code_challenge was specified in the original request) – The base64 URL-encoded representation of the unhashed, random string that was used to generate the PKCE code_challenge in the original request.
*/
        }
    }
}
