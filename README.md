# Codevos.Net.Testing

Provides an easy to use framework for writing .NET core automated (service level) tests.

## Key features

- TestWebHost to run tests against
  
  - Use single test webserver instance for all tests and create a new instance for an isolated test
  - Get services from IServiceProvider inside your tests
  - Custom application initialization (requires call in Startup class)

## Custom application initialization from Startup class

To enable custom application initialization from the TestWebHostFactory, add the following call to your Startup class:

```csharp
public void Configure(IApplicationBuilder app)
{
    app.ApplicationServices.GetService<Action<IApplicationBuilder>>()?.Invoke(app);
}
```


## Example TestWebHostFactory implementation

```csharp
public class TestWebHostFactory : Codevos.Net.Testing.TestWebHostFactory<Startup>
{
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        // you can replace already registered services here with other implementations (e.g. in-memory)
        services
	        .AddInMemoryStorage(new Uri("https://fakestorage.mycompany.com"))
	        .AddSingleton(new InMemoryServiceBus())
	        .AddSingleton<IServiceBusClientFactory, InMemoryServiceBusClientFactory>();
        // etc.
    }
    
    protected override void ConfigureTestAppConfiguration(WebHostBuilderContext webHostBuilderContext, IConfigurationBuilder configurationBuilder)
    {
        base.ConfigureTestAppConfiguration(webHostBuilderContext, configurationBuilder);
        
        configurationBuilder
            .AddJsonFile(Path.Combine(ConfigurationFolder, $"customtestsettings.json"), true, false);
    }
}
```


## Mocking the authenticated user in TestWebHostFactory


```csharp
public class TestWebHostFactory : Codevos.Net.Testing.TestWebHostFactory<Startup>
{  
    protected override void ConfigureTestServices(IServiceCollection services)
    {
        services
            .AddAuthentication(TestAuthenticationDefaults.AuthenticationScheme)
            .AddTest(options =>
            {
                options.GetUser = GetTestUser;
            });
    }

	private ClaimsPrincipal GetTestUser(string authenticationScheme, HttpContext httpContext)
    {
        var issuedAt = (long)DateTime.UtcNow.Subtract(DateTime.UnixEpoch).TotalSeconds;
        var expiresAt = issuedAt + 60; // valid for 1 minute

        return new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim("sub", "12345"), // subject
                new Claim("iss", "http://localhost"), // issuer
                new Claim("nbf", issuedAt.ToString()), // not-before
                new Claim("exp", expiresAt.ToString()), // expires
            },
            JwtBearerDefaults.AuthenticationScheme)
        );
    }
}
```