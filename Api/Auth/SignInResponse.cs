namespace Mutara.Web.Api.Auth
{
    public class SignInResponse
    {
        public string? SessionId { get; set; }
        public string? AccessToken { get; set; }
        public string? IdToken { get; set; }
        public string? RefreshToken { get; set; }
        public string? TokenType { get; set; }
        public int ExpiresIn { get; set; }
    }
}