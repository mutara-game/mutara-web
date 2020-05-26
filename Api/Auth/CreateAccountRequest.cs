using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Mutara.Web.Api
{
    public class CreateAccountRequest
    {
        [BindRequired]
        public Guid UserId { get; set; }

        [BindRequired] 
        public string Password { get; set; }
    }
}