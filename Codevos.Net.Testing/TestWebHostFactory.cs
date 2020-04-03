using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codevos.Net.Testing
{
    /// <summary>
    /// Test webhost factory.
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public abstract class TestWebHostFactory<TStartup> : IDisposable
        where TStartup : class
    {
        /// <summary>
        /// Gets the path to the folder containing the (.json) configuration files.
        /// </summary>
        protected string ConfigurationFolder { get; }

        private const string VariableNameAspNetCoreEnvironment = "ASPNETCORE_ENVIRONMENT";

        private readonly Stack<WebApplicationFactory<TStartup>> WebApplicationFactories;
        private readonly IDictionary<string, TestWebHost<TStartup>> TestWebHosts;

        private WebApplicationFactory<TStartup> webApplicationFactory;
        private WebApplicationFactory<TStartup> WebApplicationFactory
        {
            get
            {
                if (webApplicationFactory == null)
                {
                    webApplicationFactory = new WebApplicationFactory<TStartup>();
                    WebApplicationFactories.Push(webApplicationFactory);
                }

                return webApplicationFactory;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TestWebHostFactory{TStartup}"/> class.
        /// </summary>
        public TestWebHostFactory()
        {
            var environment = Environment.GetEnvironmentVariable(VariableNameAspNetCoreEnvironment);
            if (string.IsNullOrWhiteSpace(environment)) Environment.SetEnvironmentVariable(VariableNameAspNetCoreEnvironment, "Development");

            ConfigurationFolder = Path.Combine(Path.GetDirectoryName(GetType().Assembly.Location), "json", "config", "testsettings");
            WebApplicationFactories = new Stack<WebApplicationFactory<TStartup>>();
            TestWebHosts = new Dictionary<string, TestWebHost<TStartup>>();
        }

        /// <summary>
        /// Creates a new instance of the <see cref="TestWebHostFactory{TStartup}"/> class and stores it under the given key.
        /// If the key already exists, the existing <see cref="TestWebHostFactory{TStartup}"/> is returned.
        /// </summary>
        /// <param name="key">The key to identify the <see cref="TestWebHost{TStartup}"/> with.</param>
        /// <param name="setup">Optional test webhost setup.</param>
        /// <returns>The created (or already created) <see cref="TestWebHost{TStartup}"/>.</returns>
        public TestWebHost<TStartup> Create(string key, TestWebHostSetup setup = null)
        {
            if (TestWebHosts.TryGetValue(key, out TestWebHost<TStartup> testWebHost))
            {
                return testWebHost;
            }

            var webApplicationFactoryWithWebHostBuilder = WebApplicationFactory.WithWebHostBuilder(builder =>
            {
                builder
                    .ConfigureAppConfiguration((webHostBuilderContext, configurationBuilder) =>
                    {
                        ConfigureTestAppConfiguration(webHostBuilderContext, configurationBuilder);
                        setup?.ConfigurationSetup?.Invoke(webHostBuilderContext, configurationBuilder);
                    })
                    .ConfigureTestServices(services =>
                    {
                        ConfigureTestServices(services);
                        setup?.ServicesSetup?.Invoke(services);

                        services
                            .AddSingleton<Action<IApplicationBuilder>>(x => app =>
                            {
                                ConfigureTestApp(x, app);
                                setup?.AppSetup?.Invoke(x, app);
                            });
                    });
            });

            WebApplicationFactories.Push(webApplicationFactoryWithWebHostBuilder);

            testWebHost = new TestWebHost<TStartup>(webApplicationFactoryWithWebHostBuilder);
            TestWebHosts.Add(key, testWebHost);

            return testWebHost;
        }

        /// <summary>
        /// Configures the test application configuration.
        /// </summary>
        /// <param name="webHostBuilderContext">The web host builder context.</param>
        /// <param name="configurationBuilder">The configuration builder.</param>
        protected virtual void ConfigureTestAppConfiguration(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .AddJsonFile(Path.Combine(ConfigurationFolder, $"testsettings.json"), true, false)
                .AddJsonFile(Path.Combine(ConfigurationFolder, $"testsettings.{webHostBuilderContext.HostingEnvironment.EnvironmentName}.json"), true, false);
        }

        /// <summary>
        /// Configures test services.
        /// Use this method to replace existing services with (test) implementations and to add additional services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The service collection.</param>
        protected abstract void ConfigureTestServices(IServiceCollection services);

        /// <summary>
        /// Configures test app.
        /// Use this method add additional login to <see cref="IApplicationBuilder"/>.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="app">The application builder.</param>
        protected virtual void ConfigureTestApp(IServiceProvider serviceProvider, IApplicationBuilder app)
        {
            // override in child classes
        }

        /// <summary>
        /// Disposes the test webhost factory.
        /// </summary>
        public void Dispose()
        {
            foreach (var webHostKey in TestWebHosts.Keys.ToArray())
            {
                var testWebHost = TestWebHosts[webHostKey];
                TestWebHosts.Remove(webHostKey);
                testWebHost.Dispose();
            }

            while (WebApplicationFactories.Count > 0)
            {
                var webApplicationFactoryWithWebHostBuilder = WebApplicationFactories.Pop();
                webApplicationFactoryWithWebHostBuilder.Dispose();
            }
        }
    }
}