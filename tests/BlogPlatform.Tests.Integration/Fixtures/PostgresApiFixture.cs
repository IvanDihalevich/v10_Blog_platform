using Testcontainers.PostgreSql;

namespace BlogPlatform.Tests.Integration.Fixtures;

public sealed class PostgresApiFixture : IAsyncLifetime
{
    private PostgreSqlContainer? _container;

    public string ConnectionString => _container!.GetConnectionString();

    public async Task InitializeAsync()
    {
        _container = new PostgreSqlBuilder().Build();
        await _container.StartAsync();
    }

    public async Task DisposeAsync()
    {
        if (_container is not null)
            await _container.DisposeAsync();
    }
}
