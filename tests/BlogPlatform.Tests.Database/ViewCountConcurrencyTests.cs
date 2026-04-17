using BlogPlatform.Domain.Entities;
using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Tests.Database.Fixtures;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Tests.Database;

[Collection("PostgresDb")]
public sealed class ViewCountConcurrencyTests
{
    private readonly string _cs;

    public ViewCountConcurrencyTests(PostgresDbFixture f)
    {
        _cs = f.ConnectionString;
    }

    [Fact]
    public async Task Concurrent_increments_yield_expected_total()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>().UseNpgsql(_cs).Options;

        var slug = "concurrency-slug-" + Guid.NewGuid().ToString("n");
        int id;
        await using (var db = new BlogDbContext(options))
        {
            await db.Database.MigrateAsync();
            db.Posts.Add(new Post
            {
                Title = "C",
                Content = "z",
                AuthorName = "u",
                Slug = slug,
                IsPublished = true,
                PublishedAt = DateTime.UtcNow,
                ViewCount = 0
            });
            await db.SaveChangesAsync();
            id = await db.Posts.Where(p => p.Slug == slug).Select(p => p.Id).SingleAsync();
        }

        const int workers = 40;
        var tasks = Enumerable.Range(0, workers).Select(_ => Task.Run(async () =>
        {
            await using var ctx = new BlogDbContext(options);
            await ctx.Posts.Where(p => p.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1));
        }));

        await Task.WhenAll(tasks);

        await using (var verify = new BlogDbContext(options))
        {
            var final = await verify.Posts.AsNoTracking().Where(p => p.Id == id).Select(p => p.ViewCount).SingleAsync();
            Assert.Equal(workers, final);
        }
    }
}
