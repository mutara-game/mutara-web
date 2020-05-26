using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mutara.Web.Api.Auth
{
    // TODO add support for facebook, google, etc...
    public class SignInRequest
    {
        
        [BindRequired] public Guid UserId { get; set; }

        [BindRequired] public string Password { get; set; } = null!;
        
        public string? ClientVersion { get; set; }
    }
}