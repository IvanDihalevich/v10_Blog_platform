using BlogPlatform.Domain.Entities;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Tests.Database.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Tests.Database;

[Collection("PostgresDb")]
public sealed class PostTagMappingTests
{
    private readonly string _cs;

    public PostTagMappingTests(PostgresDbFixture f)
    {
        _cs = f.ConnectionString;
    }

    [Fact]
    public async Task Many_to_many_maps_post_tags_and_round_trips()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>().UseNpgsql(_cs).Options;
        await using var db = new BlogDbContext(options);
        await db.Database.MigrateAsync();

        var t1 = new Tag { Name = "alpha-" + Guid.NewGuid().ToString("n") };
        var t2 = new Tag { Name = "beta-" + Guid.NewGuid().ToString("n") };
        db.Tags.AddRange(t1, t2);
        await db.SaveChangesAsync();

        var post = new Post
        {
            Title = "Tagged",
            Content = "Body",
            AuthorName = "Author",
            Slug = "tagged-" + Guid.NewGuid().ToString("n"),
            IsPublished = true,
            PublishedAt = DateTime.UtcNow,
            ViewCount = 0
        };
        db.Posts.Add(post);
        await db.SaveChangesAsync();

        db.PostTags.AddRange(
            new PostTag { PostId = post.Id, TagId = t1.Id },
            new PostTag { PostId = post.Id, TagId = t2.Id });
        await db.SaveChangesAsync();

        var loaded = await db.Posts
            .AsNoTracking()
            .Include(p => p.PostTags)
            .ThenInclude(pt => pt.Tag)
            .FirstAsync(p => p.Id == post.Id);

        Assert.Equal(2, loaded.PostTags.Count);
        Assert.Contains(loaded.PostTags, pt => pt.Tag.Name == t1.Name);
        Assert.Contains(loaded.PostTags, pt => pt.Tag.Name == t2.Name);
    }
}
