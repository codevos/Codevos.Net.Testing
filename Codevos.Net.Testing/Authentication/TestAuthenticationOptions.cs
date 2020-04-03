using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Codevos.Net.Testing.Authentication
{
    /// <summary>
    /// Test authentication options.
    /// </summary>
    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        /// <summary>
        /// Gets or sets the function to invoke for getting the authenticated user.
        /// </summary>
        public Func<string, HttpContext, ClaimsPrincipal> GetUser { get; set; }
    }
}
