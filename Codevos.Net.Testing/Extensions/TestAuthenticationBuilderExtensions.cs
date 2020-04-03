using System;
using Codevos.Net.Testing.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Provides <see cref="AuthenticationBuilder"/> extension methods for authentication mocking.
    /// </summary>
    public static class TestAuthenticationBuilderExtensions
    {
        /// <summary>
        /// Adds test authentication using <see cref="TestAuthenticationDefaults.AuthenticationScheme"/> as the authentication scheme name.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="configureOptions">The method to configure the <see cref="TestAuthenticationOptions"/> with.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> to use for further configuration.</returns>
        public static AuthenticationBuilder AddTest(this AuthenticationBuilder builder, Action<TestAuthenticationOptions> configureOptions)
        {
            return AddTest(builder, TestAuthenticationDefaults.AuthenticationScheme, configureOptions);
        }

        /// <summary>
        /// Adds test authentication.
        /// </summary>
        /// <param name="builder">The <see cref="AuthenticationBuilder"/>.</param>
        /// <param name="authenticationScheme">The authentication scheme name.</param>
        /// <param name="configureOptions">The method to configure the <see cref="TestAuthenticationOptions"/> with.</param>
        /// <returns>The <see cref="AuthenticationBuilder"/> to use for further configuration.</returns>
        public static AuthenticationBuilder AddTest(this AuthenticationBuilder builder, string authenticationScheme, Action<TestAuthenticationOptions> configureOptions)
        {
            builder.Services.AddSingleton<IPostConfigureOptions<TestAuthenticationOptions>, TestAuthenticationPostConfigureOptions>();
            builder.AddScheme<TestAuthenticationOptions, TestAuthenticationHandler>(authenticationScheme, configureOptions);
            return builder;
        }
    }
}