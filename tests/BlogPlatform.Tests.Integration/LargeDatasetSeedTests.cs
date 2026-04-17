using BlogPlatform.Infrastructure.Data;
using BlogPlatform.Tests.Integration.Fixtures;
using BlogPlatform.Tests.Integration.Seeding;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Tests.Integration;

[Collection("Integration")]
public sealed class LargeDatasetSeedTests
{
    private readonly string _connectionString;

    public LargeDatasetSeedTests(PostgresApiFixture pg)
    {
        _connectionString = pg.ConnectionString;
    }

    [Fact]
    public async Task Seed_inserts_at_least_10000_rows_across_entities()
    {
        var options = new DbContextOptionsBuilder<BlogDbContext>()
            .UseNpgsql(_connectionString)
            .Options;

        await using (var ctx = new BlogDbContext(options))
        {
            await ctx.Database.MigrateAsync();
            var inserted = await LargeDatasetSeeder.SeedAsync(ctx);
            Assert.True(inserted >= 10_000);
            Assert.True(await ctx.Posts.CountAsync() >= 2_000);
            Assert.True(await ctx.Tags.CountAsync() >= 600);
            Assert.True(await ctx.Comments.CountAsync() >= 7_000);
            Assert.True(await ctx.PostTags.CountAsync() >= 1_000);
        }

        await using (var verify = new BlogDbContext(options))
        {
            var total =
                await verify.Posts.CountAsync()
                + await verify.Tags.CountAsync()
                + await verify.Comments.CountAsync()
                + await verify.PostTags.CountAsync();
            Assert.True(total >= 10_000);
        }
    }
}
