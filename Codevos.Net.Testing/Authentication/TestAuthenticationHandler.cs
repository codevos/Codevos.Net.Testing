using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codevos.Net.Testing.Authentication
{
    /// <summary>
    /// Test authentication handler.
    /// </summary>
    public class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestAuthenticationHandler"/> class.
        /// </summary>
        /// <param name="options">The test authentication options.</param>
        /// <param name="loggerFactory">The logger factory.</param>
        /// <param name="encoder">The URL encoder.</param>
        /// <param name="clock">The system clock.</param>
        public TestAuthenticationHandler(
            IOptionsMonitor<TestAuthenticationOptions> options,
            ILoggerFactory loggerFactory,
            UrlEncoder encoder,
            ISystemClock clock)
        : base(options, loggerFactory, encoder, clock)
        {
        }

        /// <summary>
        /// Handles the authentication as configured in the <see cref="Options"/>.
        /// </summary>
        /// <returns>The authentication result.</returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var user = Options.GetUser?.Invoke(Scheme.Name, Context);

            if (user == null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(user, Scheme.Name)));
        }
    }
}