using BlogPlatform.Domain.Entities;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Tests.Database.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Tests.Database;

[Collection("PostgresDb")]
public sealed class SlugUniquenessTests
{
    private readonly string _cs;

    public SlugUniquenessTests(PostgresDbFixture f)
    {
        _cs = f.ConnectionString;
    }

    [Fact]
    public async Task Duplicate_slug_is_rejected_by_database()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>().UseNpgsql(_cs).Options;
        await using var db = new BlogDbContext(options);
        await db.Database.MigrateAsync();

        db.Posts.Add(new Post
        {
            Title = "A",
            Content = "x",
            AuthorName = "a",
            Slug = "same-slug",
            IsPublished = false,
            ViewCount = 0
        });
        await db.SaveChangesAsync();

        db.Posts.Add(new Post
        {
            Title = "B",
            Content = "y",
            AuthorName = "b",
            Slug = "same-slug",
            IsPublished = false,
            ViewCount = 0
        });

        await Assert.ThrowsAsync<DbUpdateException>(() => db.SaveChangesAsync());
    }
}
