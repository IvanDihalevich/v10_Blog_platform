using AutoFixture;
using BlogPlatform.Api.Services;
using BlogPlatform.Domain.Entities;

namespace BlogPlatform.Tests.Unit.EntityTestData;

public sealed class BlogEntityAutoFixtureRulesTests
{
    [Fact]
    public void Post_auto_generated_fields_are_filled_explicit_business_fields_are_set_in_test()
    {
        var fixture = BlogEntityFixtureFactory.CreateFixture();
        var post = fixture.Create<Post>();

        Assert.False(string.IsNullOrWhiteSpace(post.Title));
        Assert.False(string.IsNullOrWhiteSpace(post.Content));
        Assert.False(string.IsNullOrWhiteSpace(post.AuthorName));

        post.Slug = SlugNormalizer.FromTitle(post.Title);
        post.IsPublished = false;
        post.ViewCount = 0;
        post.PublishedAt = null;

        Assert.NotEmpty(post.Slug);
        Assert.False(post.IsPublished);
        Assert.Equal(0, post.ViewCount);
    }

    [Fact]
    public void Comment_auto_generated_fields_explicit_post_and_approval()
    {
        var fixture = BlogEntityFixtureFactory.CreateFixture();
        var comment = fixture.Create<Comment>();

        Assert.False(string.IsNullOrWhiteSpace(comment.AuthorName));
        Assert.False(string.IsNullOrWhiteSpace(comment.Content));

        comment.PostId = 42;
        comment.IsApproved = false;

        Assert.Equal(42, comment.PostId);
        Assert.False(comment.IsApproved);
    }

    [Fact]
    public void Tag_name_from_fixture_is_valid_distinct_entity()
    {
        var fixture = BlogEntityFixtureFactory.CreateFixture();
        var tag = fixture.Create<Tag>();

        Assert.False(string.IsNullOrWhiteSpace(tag.Name));
    }
}
