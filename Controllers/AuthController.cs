using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Mutara.Web.Api;
using Mutara.Web.Api.Auth;
using Mutara.Web.Services;

namespace Mutara.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly ILogger<AuthController> logger;
        private CognitoService cognitoService;

        public AuthController(ILogger<AuthController> logger, 
            ConfigClient configClient,
            CognitoService cognitoService)
        {
            this.logger = logger;
            this.cognitoService = cognitoService;
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn(SignInRequest request)
        {
            logger.LogInformation("sign in request received");
            try
            {
                AuthFlowResponse authResponse = await cognitoService.SignIn(request.UserId, request.Password);
                
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
                logger.LogWarning(e, "user {UserId} failed to authenticate with cognito", request.UserId);
                return Unauthorized(new ApiResponse(401, "authentication failed"));
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Unexpected exception from user {request.UserId} failing to authnticate with cognito");
                throw;
            }
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount(CreateAccountRequest request)
        {
            try
            {
                logger.LogInformation("CreateAccount Request Received");

                var signupResponse = await cognitoService.SignUp(request.UserId, request.Password);
                return Ok(new ApiOkResponse<CreateAccountResponse>(
                    new CreateAccountResponse
                    {
                        UserSub = signupResponse.UserSub,
                        UserConfirmed = signupResponse.UserConfirmed,
                    }
                ));
            }
            catch (UsernameExistsException e)
            {
                logger.LogWarning(e, "CreateRequest for user already exists: {0}", request.UserId);
                return BadRequest("UserId already exists");
            }
            catch (AmazonCognitoIdentityProviderException e)
            {
                logger.LogError(e, "Error from Cognito for user {0}", request.UserId);
                return BadRequest(e.ToString());
            }
        }
    }
}
