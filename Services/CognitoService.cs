using System;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Extensions.CognitoAuthentication;
using Microsoft.Extensions.Logging;

namespace Mutara.Web.Services
{
    public class CognitoService
    {
        private readonly ILogger<CognitoService> logger;
        private readonly AmazonCognitoIdentityProviderClient cognitoIpClient;

        // NOTE: changing these values "live" doesn't update automatically - requires a restart
        private readonly string clientId;
        private readonly string cognitoPoolId;

        public CognitoService(ILogger<CognitoService> logger, ConfigClient configClient,
            AmazonCognitoIdentityProviderClient cognitoIpClient)
        {
            this.logger = logger;
            this.cognitoIpClient = cognitoIpClient;

            this.clientId = configClient.GetValue("secrets.yaml", "cognito/clientId").GetAwaiter().GetResult();
            this.cognitoPoolId = configClient.GetValue("secrets.yaml", "cognito/poolId").GetAwaiter().GetResult();
        }


        public async Task<AuthFlowResponse> SignIn(Guid userId, string password)
        {
            var userPool = new CognitoUserPool(cognitoPoolId, clientId, cognitoIpClient);
            var user = new CognitoUser(userId.ToString(), clientId, userPool, cognitoIpClient);

            return await user.StartWithSrpAuthAsync(new InitiateSrpAuthRequest()
                {
                    Password = password
                })
                .ConfigureAwait(false);
        }

        
        public async Task<SignUpResponse> SignUp(Guid userId, string password)
        {
            var signupRequest = new SignUpRequest
            {
                ClientId = clientId,
                Password = password,
                Username = userId.ToString()
            };
            return await cognitoIpClient.SignUpAsync(signupRequest);
        }
    }
}
