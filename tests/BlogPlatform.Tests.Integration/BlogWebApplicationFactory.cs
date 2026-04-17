using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BlogPlatform.Tests.Integration;

public sealed class BlogWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;

    public BlogWebApplicationFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.UseSetting("ConnectionStrings:DefaultConnection", _connectionString);
    }
}
