using System.Net.Http.Json;
using System.Text.Json;
using BlogPlatform.Tests.Integration.Fixtures;

namespace BlogPlatform.Tests.Integration;

[Collection("Integration")]
public sealed class CommentModerationIntegrationTests : IDisposable
{
    private readonly BlogWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public CommentModerationIntegrationTests(PostgresApiFixture pg)
    {
        _factory = new BlogWebApplicationFactory(pg.ConnectionString);
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    [Fact]
    public async Task Comment_hidden_until_approved_then_visible()
    {
        var key = Guid.NewGuid().ToString("n");
        var title = $"Post {key}";

        var create = await _client.PostAsJsonAsync(
            "/api/posts",
            new { title, content = "Body", authorName = "Writer" });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<JsonElement>();
        var id = created.GetProperty("id").GetInt32();

        await _client.PatchAsync($"/api/posts/{id}/publish", null);

        var commentRes = await _client.PostAsJsonAsync(
            $"/api/posts/{id}/comments",
            new { authorName = "Reader", content = "Needs approval" });
        commentRes.EnsureSuccessStatusCode();
        var comment = await commentRes.Content.ReadFromJsonAsync<JsonElement>();
        var commentId = comment.GetProperty("id").GetInt32();

        var before = await _client.GetFromJsonAsync<JsonElement>($"/api/posts/{id}/comments");
        Assert.Equal(0, before.GetArrayLength());

        var approve = await _client.PatchAsync($"/api/comments/{commentId}/approve", null);
        approve.EnsureSuccessStatusCode();

        var after = await _client.GetFromJsonAsync<JsonElement>($"/api/posts/{id}/comments");
        Assert.Equal(1, after.GetArrayLength());
        Assert.Equal("Needs approval", after[0].GetProperty("content").GetString());
    }
}
