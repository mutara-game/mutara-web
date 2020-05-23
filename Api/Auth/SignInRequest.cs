using System;

namespace Mutara.Web.Api
{
    // TODO add support for facebook, google, etc...
    public class SignInRequest
    {
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? ClientVersion { get; set; }
    }
}