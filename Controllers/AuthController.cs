using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using Mutara.Web.Api;
using Mutara.Web.Services;

namespace Mutara.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private readonly AmazonCognitoIdentityProviderClient cognitoIpClient;

        // NOTE: changing these values "live" doesn't update automatically - requires a restart
        private readonly string clientId;
        private readonly string cognitoPoolId;

        public AuthController(ILogger<AuthController> logger, ConfigClient configClient,
            AmazonCognitoIdentityProviderClient cognitoIpClient)
        {
            this.logger = logger;
            this.cognitoIpClient = cognitoIpClient;

            this.clientId = configClient.GetValue("secrets.yaml", "cognito/clientId").GetAwaiter().GetResult();
            this.cognitoPoolId = configClient.GetValue("secrets.yaml", "cognito/poolId").GetAwaiter().GetResult();
        }

        [HttpPost("signin")]
        public async Task<SignInResponse> SignIn(SignInRequest request)
        {
            var userPool = new CognitoUserPool(cognitoPoolId, clientId, cognitoIpClient);
            var user = new CognitoUser(request.UserName, clientId, userPool, cognitoIpClient);

            AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
                {
                    Password = request.Password
                })
                .ConfigureAwait(false);

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
