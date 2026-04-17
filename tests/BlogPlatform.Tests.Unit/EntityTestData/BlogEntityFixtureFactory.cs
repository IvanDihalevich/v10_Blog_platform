using AutoFixture;
using BlogPlatform.Domain.Entities;

namespace BlogPlatform.Tests.Unit.EntityTestData;

/// <summary>
/// AutoFixture генерує лише «некритичні» поля з завдання (Title, Content, AuthorName, PublishedAt для Post тощо).
/// Slug, IsPublished, ViewCount, IsApproved, PostId задаються явно в тестах.
/// </summary>
public static class BlogEntityFixtureFactory
{
    public static Fixture CreateFixture()
    {
        var fixture = new Fixture();
        fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        fixture.Customize<Post>(c => c
            .Without(p => p.Id)
            .Without(p => p.Slug)
            .Without(p => p.IsPublished)
            .Without(p => p.ViewCount)
            .Without(p => p.Comments)
            .Without(p => p.PostTags));

        fixture.Customize<Comment>(c => c
            .Without(x => x.Id)
            .Without(x => x.PostId)
            .Without(x => x.IsApproved)
            .Without(x => x.Post));

        fixture.Customize<Tag>(c => c
            .Without(t => t.Id)
            .Without(t => t.PostTags));

        return fixture;
    }
}
