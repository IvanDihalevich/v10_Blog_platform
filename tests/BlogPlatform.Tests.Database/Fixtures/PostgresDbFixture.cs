using Testcontainers.PostgreSql;

namespace BlogPlatform.Tests.Database.Fixtures;

public sealed class PostgresDbFixture : IAsyncLifetime
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
