using BlogPlatform.Domain.Entities;
using BlogPlatform.Infrastructure.Data;
using Bogus;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Tests.Integration.Seeding;

public static class LargeDatasetSeeder
{
    /// <summary>
    /// Inserts at least 10,000 rows across Posts, Comments, Tags, and PostTags using Bogus for natural text and dates.
    /// Explicit control fields (slug uniqueness, flags, FKs) are set deterministically.
    /// </summary>
    public static async Task<int> SeedAsync(BlogDbContext db, CancellationToken cancellationToken = default)
    {
        var postFaker = new Faker<Post>()
            .RuleFor(p => p.Title, f => f.Lorem.Sentence(3, 8))
            .RuleFor(p => p.Content, f => f.Lorem.Paragraphs(2, "\n\n"))
            .RuleFor(p => p.AuthorName, f => f.Name.FullName())
            .RuleFor(p => p.PublishedAt, f => DateTime.SpecifyKind(f.Date.Past(400), DateTimeKind.Utc));

        var tagFaker = new Faker<Tag>()
            .RuleFor(t => t.Name, f => $"{f.Random.Word()}-{f.Random.AlphaNumeric(8)}");

        const int tagCount = 600;
        var tags = tagFaker.Generate(tagCount);
        for (var i = 0; i < tags.Count; i++)
            tags[i].Name = $"tag-{i}-{tags[i].Name}";

        db.Tags.AddRange(tags);
        await db.SaveChangesAsync(cancellationToken);

        const int postCount = 2000;
        var posts = postFaker.Generate(postCount);
        for (var i = 0; i < posts.Count; i++)
        {
            var p = posts[i];
            p.Slug = $"seed-post-{i}";
            p.IsPublished = i == 0 || i % 3 != 0;
            p.ViewCount = i;
            p.PublishedAt = p.IsPublished ? p.PublishedAt : null;
        }

        db.Posts.AddRange(posts);
        await db.SaveChangesAsync(cancellationToken);

        var random = new Random(12345);
        var links = new List<PostTag>();
        var seenPairs = new HashSet<(int PostId, int TagId)>();
        foreach (var post in posts)
        {
            var n = random.Next(1, 4);
            foreach (var tag in tags.OrderBy(_ => random.Next()).Take(n))
            {
                var key = (post.Id, tag.Id);
                if (seenPairs.Add(key))
                    links.Add(new PostTag { PostId = post.Id, TagId = tag.Id });
            }
        }

        db.PostTags.AddRange(links);
        await db.SaveChangesAsync(cancellationToken);

        var commentFaker = new Faker<Comment>()
            .RuleFor(c => c.AuthorName, f => f.Internet.UserName())
            .RuleFor(c => c.Content, f => f.Lorem.Paragraph())
            .RuleFor(c => c.CreatedAt, f => DateTime.SpecifyKind(f.Date.Recent(120), DateTimeKind.Utc));

        const int commentCount = 7500;
        var comments = commentFaker.Generate(commentCount);
        for (var i = 0; i < comments.Count; i++)
        {
            var c = comments[i];
            c.PostId = posts[random.Next(posts.Count)].Id;
            c.IsApproved = i % 4 != 0;
        }

        db.Comments.AddRange(comments);
        await db.SaveChangesAsync(cancellationToken);

        return tagCount + postCount + links.Count + commentCount;
    }
}
