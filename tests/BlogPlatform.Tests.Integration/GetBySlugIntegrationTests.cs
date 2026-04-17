using System.Net.Http.Json;
using System.Text.Json;
using BlogPlatform.Tests.Integration.Fixtures;

namespace BlogPlatform.Tests.Integration;

[Collection("Integration")]
public sealed class GetBySlugIntegrationTests : IDisposable
{
    private readonly BlogWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GetBySlugIntegrationTests(PostgresApiFixture pg)
    {
        _factory = new BlogWebApplicationFactory(pg.ConnectionString);
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Get_by_slug_increments_view_count_each_request()
    {
        var key = Guid.NewGuid().ToString("n");
        var title = $"Slug test {key}";

        var create = await _client.PostAsJsonAsync(
            "/api/posts",
            new { title, content = "X", authorName = "A" });
        var created = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();
        var slug = created.GetProperty("slug").GetString()!;

        await _client.PatchAsync($"/api/posts/{id}/publish", null);

        await _client.GetAsync($"/api/posts/{slug}");
        await _client.GetAsync($"/api/posts/{slug}");
        var third = await _client.GetFromJsonAsync<JsonElement>($"/api/posts/{slug}");

        Assert.Equal(3, third.GetProperty("viewCount").GetInt32());
    }
}
