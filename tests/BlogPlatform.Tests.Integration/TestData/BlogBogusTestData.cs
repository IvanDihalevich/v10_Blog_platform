using BlogPlatform.Domain.Entities;
using Bogus;

namespace BlogPlatform.Tests.Integration.TestData;

/// <summary>
/// Builds entities with Bogus for natural text and timestamps; callers set Slug, flags, FKs explicitly.
/// </summary>
public static class BlogBogusTestData
{
    private static readonly Faker Faker = new();

    public static Post CreatePost(Action<Post>? configure = null)
    {
        var p = new Faker<Post>()
            .CustomInstantiator(_ => new Post())
            .RuleFor(x => x.Title, f => f.Lorem.Sentence(4))
            .RuleFor(x => x.Content, f => f.Lorem.Paragraphs(2))
            .RuleFor(x => x.AuthorName, f => f.Name.FullName())
            .RuleFor(x => x.PublishedAt, f => DateTime.SpecifyKind(f.Date.Past(200), DateTimeKind.Utc))
            .Generate();

        configure?.Invoke(p);
        return p;
    }

    public static Comment CreateComment(int postId, Action<Comment>? configure = null)
    {
        var c = new Faker<Comment>()
            .CustomInstantiator(_ => new Comment())
            .RuleFor(x => x.AuthorName, f => f.Internet.UserName())
            .RuleFor(x => x.Content, f => f.Lorem.Sentence(12))
            .RuleFor(x => x.CreatedAt, f => DateTime.SpecifyKind(f.Date.Recent(30), DateTimeKind.Utc))
            .Generate();

        c.PostId = postId;
        configure?.Invoke(c);
        return c;
    }

    public static Tag CreateTag(Action<Tag>? configure = null)
    {
        var t = new Faker<Tag>()
            .CustomInstantiator(_ => new Tag())
            .RuleFor(x => x.Name, f => f.Commerce.Department() + "-" + f.Random.AlphaNumeric(6))
            .Generate();

        configure?.Invoke(t);
        return t;
    }
}
