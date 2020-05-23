using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
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
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            logger.LogInformation("sign in request received");
            var userPool = new CognitoUserPool(cognitoPoolId, clientId, cognitoIpClient);
            var user = new CognitoUser(request.UserName, clientId, userPool, cognitoIpClient);

            try
            {
                AuthFlowResponse authResponse = await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
                    {
                        Password = request.Password
                    })
                    .ConfigureAwait(false);

                return Ok(new ApiOkResponse<SignInResponse>(new SignInResponse()
                {
                    SessionId = authResponse.SessionID,
                    AccessToken = authResponse.AuthenticationResult.AccessToken,
                    IdToken = authResponse.AuthenticationResult.IdToken,
                    RefreshToken = authResponse.AuthenticationResult.RefreshToken,
                    TokenType = authResponse.AuthenticationResult.TokenType,
                    ExpiresIn = authResponse.AuthenticationResult.ExpiresIn
                }));
            }
            catch (NotAuthorizedException e)
            {
                logger.LogWarning(e, $"user {request.UserName} failed to authenticate with cognito");
                return Unauthorized(new ApiResponse(401, "authentication failed"));
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unexpected exception from user {request.UserName} failing to authnticate with cognito");
                throw;
            }
        }
    }
}
