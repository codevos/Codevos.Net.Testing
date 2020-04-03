using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Codevos.Net.Testing
{
    /// <summary>
    /// Test webhost class.
    /// </summary>
    /// <typeparam name="TStartup"></typeparam>
    public class TestWebHost<TStartup> : IDisposable
        where TStartup : class
    {
        /// <summary>
        /// Gets the test server.
        /// </summary>
        public TestServer Server { get; }

        /// <summary>
        /// Gets the service provider to get registered services from in tests.
        /// </summary>
        public IServiceProvider Services { get; }

        /// <summary>
        /// Gets the HTTP client to use for making calls to the webhost.
        /// </summary>
        public HttpClient HttpClient { get; }

        private JsonSerializerOptions _jsonSerializerOptions;
        /// <summary>
        /// Gets the JSON serializer setting which are registered in the <typeparamref name="TStartup"/> class.
        /// </summary>
        public JsonSerializerOptions JsonSerializerOptions => _jsonSerializerOptions ?? (_jsonSerializerOptions = Services?.GetRequiredService<IOptions<JsonOptions>>()?.Value?.JsonSerializerOptions ?? new JsonSerializerOptions());

        /// <summary>
        /// Initializes a new instance of the <see cref="TestWebHost{TStartup}"/> class.
        /// </summary>
        /// <param name="webApplicationFactory">The web application factory.</param>
        public TestWebHost(WebApplicationFactory<TStartup> webApplicationFactory)
        {
            HttpClient = webApplicationFactory.CreateClient();
            HttpClient.Timeout = TimeSpan.FromMinutes(1);
            Server = webApplicationFactory.Server;
            Services = webApplicationFactory.Server?.Services;
        }

        /// <summary>
        /// Disposes the test webhost.
        /// </summary>
        public void Dispose()
        {
            HttpClient.Dispose();
        }
    }
}