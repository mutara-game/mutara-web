using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Amazon.Runtime;
using Mutara.Web.Api;
using Mutara.Web.Services;

namespace Mutara.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly HttpClient client;

        private readonly string clientId;
        private readonly string redirectUri;
        private readonly string cognitoDomain;
        private readonly string cognitoPoolId;

        public AuthController(ILogger<AuthController> logger, ConfigClient configClient)
        {
            this.logger = logger;
            this.client = new HttpClient();

            this.clientId = configClient.GetValue("secrets.yaml", "cognito/clientId").GetAwaiter().GetResult();
            
            // got rid of this, it's broken...
            // string clientSecret = configClient.GetValue("secrets.yaml", "cognito/clientSecret").GetAwaiter().GetResult();
            // string userAndPasswordToken = Convert.ToBase64String(Encoding.UTF8.GetBytes(clientId + ":" + clientSecret));
            // client.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Basic {userAndPasswordToken}");

            // this is going away too
            this.cognitoDomain = configClient.GetValue("secrets.yaml", "cognito/cognitoDomain").GetAwaiter().GetResult();
            // so is this
            this.redirectUri = configClient.GetValue("secrets.yaml", "cognito/redirectUri").GetAwaiter().GetResult();
            
            this.cognitoPoolId = configClient.GetValue("secrets.yaml", "cognito/poolId").GetAwaiter().GetResult();

        }

        [HttpGet]
        public String Get()
        {
            return "Hello, GET";
        }

        /// <summary>
        /// Takes a code from cognito login web form result
        /// and turns that into token information.
        /// </summary>
        /// <param name="code"></param>
        /// <returns>the whole token, which probably isn't a great idea really.</returns>
        [HttpGet("signin")]
        public async Task<String> SignIn(string code) {
            // TODO this is now probably broken.
            // Which is OK because we're not going to have the client talk to
            // cognito itself, because cognito is definitely broken.
            
            var map = new Dictionary<string, string>
            {
                {"grant_type", "authorization_code"},
                {"code", code},
                {"client_id", clientId},
                {"redirect_uri", redirectUri}
            };
            HttpContent content = new FormUrlEncodedContent(map);

            HttpResponseMessage response = await client.PostAsync($"{cognitoDomain}/oauth2/token", content);
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

        [HttpPost("signin")]
        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            var provider = new AmazonCognitoIdentityProviderClient(new AnonymousAWSCredentials(), RegionEndpoint.USWest2);
            var userPool = new CognitoUserPool(cognitoPoolId, clientId, provider);
            var user = new CognitoUser(request.UserName, clientId, userPool, provider);
           
            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
            {
                Password = request.Password
            }).ConfigureAwait(false);

            logger.LogInformation($"{authResponse}");
            
            // TODO This is totally broken if there's any sort of error,
            // which there will be a lot of.
            return new SignInResponse()
            {
                SessionId = authResponse.SessionID,
                AccessToken = authResponse.AuthenticationResult.AccessToken,
                IdToken = authResponse.AuthenticationResult.IdToken,
                RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                TokenType = authResponse.AuthenticationResult.TokenType,
                ExpiresIn = authResponse.AuthenticationResult.ExpiresIn
            };

        }
    }
}
