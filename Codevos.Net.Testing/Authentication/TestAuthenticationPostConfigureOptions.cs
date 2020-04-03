using Microsoft.Extensions.Options;

namespace Codevos.Net.Testing.Authentication
{
    /// <summary>
    /// Test authentication post configure options.
    /// </summary>
    public class TestAuthenticationPostConfigureOptions : IPostConfigureOptions<TestAuthenticationOptions>
    {
        /// <summary>
        /// Post configures the <see cref="TestAuthenticationOptions"/>.
        /// </summary>
        /// <param name="name">The name of the options instance being configured.</param>
        /// <param name="options">The options being configured.</param>
        public void PostConfigure(string name, TestAuthenticationOptions options)
        {
        }
    }
}