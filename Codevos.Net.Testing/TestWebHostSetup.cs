using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codevos.Net.Testing
{
    /// <summary>
    /// Test webhost setup.
    /// </summary>
    public class TestWebHostSetup
    {
        /// <summary>
        /// Gets or sets the <see cref="IConfigurationBuilder"/> setup.
        /// </summary>
        public Action<WebHostBuilderContext, IConfigurationBuilder> ConfigurationSetup { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IServiceCollection"/> setup.
        /// </summary>
        public Action<IServiceCollection> ServicesSetup { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IApplicationBuilder"/> setup.
        /// </summary>
        public Action<IServiceProvider, IApplicationBuilder> AppSetup { get; set; }
    }
}
