using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using BlogPlatform.Tests.Integration.Fixtures;

namespace BlogPlatform.Tests.Integration;

[Collection("Integration")]
public sealed class DraftToPublishIntegrationTests : IDisposable
{
    private readonly BlogWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public DraftToPublishIntegrationTests(PostgresApiFixture pg)
    {
        _factory = new BlogWebApplicationFactory(pg.ConnectionString);
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Draft_to_publish_flow_allows_get_by_slug_after_publish()
    {
        var key = Guid.NewGuid().ToString("n");
        var title = $"Stable title {key}";

        var create = await _client.PostAsJsonAsync(
            "/api/posts",
            new { title, content = "", authorName = "Author" });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();
        var slug = created.GetProperty("slug").GetString();
        Assert.NotNull(slug);

        var put = await _client.PutAsJsonAsync(
            $"/api/posts/{id}",
            new { title, content = "Published body.", authorName = "Author" });
        put.EnsureSuccessStatusCode();

        var publish = await _client.PatchAsync($"/api/posts/{id}/publish", null);
        publish.EnsureSuccessStatusCode();

        var bySlug = await _client.GetAsync($"/api/posts/{slug}");
        bySlug.EnsureSuccessStatusCode();
        var post = await bySlug.Content.ReadFromJsonAsync<JsonElement>();
        Assert.True(post.GetProperty("isPublished").GetBoolean());
        Assert.Equal("Published body.", post.GetProperty("content").GetString());
    }

    [Fact]
    public async Task Publish_with_empty_content_returns_bad_request()
    {
        var key = Guid.NewGuid().ToString("n");
        var title = $"Empty {key}";

        var create = await _client.PostAsJsonAsync(
            "/api/posts",
            new { title, content = " ", authorName = "A" });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();

        await _client.PutAsJsonAsync(
            $"/api/posts/{id}",
            new { title, content = "   ", authorName = "A" });

        var publish = await _client.PatchAsync($"/api/posts/{id}/publish", null);
        Assert.Equal(HttpStatusCode.BadRequest, publish.StatusCode);
    }
}
