using AutoFixture;
using BlogPlatform.Api.Services;

namespace BlogPlatform.Tests.Unit;

public sealed class SlugNormalizerTests
{
    private readonly Fixture _fixture = new();

    [Fact]
    public void FromTitle_lowercases_and_replaces_non_alphanumeric_with_hyphens()
    {
        var slug = SlugNormalizer.FromTitle("Hello World Test");
        Assert.Equal("hello-world-test", slug);
    }

    [Fact]
    public void FromTitle_collapses_repeated_hyphens()
    {
        var slug = SlugNormalizer.FromTitle("a   b---c");
        Assert.False(slug.Contains("--", StringComparison.Ordinal));
    }

    [Fact]
    public void FromTitle_returns_post_when_title_empty_or_only_symbols()
    {
        Assert.Equal("post", SlugNormalizer.FromTitle(""));
        Assert.Equal("post", SlugNormalizer.FromTitle("   "));
        Assert.Equal("post", SlugNormalizer.FromTitle("@@@"));
    }

    [Fact]
    public void FromTitle_strips_leading_trailing_hyphen_noise()
    {
        var slug = SlugNormalizer.FromTitle("---trim---");
        Assert.False(slug.StartsWith('-'));
        Assert.False(slug.EndsWith('-'));
    }

    [Fact]
    public void FromTitle_handles_auto_generated_titles_from_fixture()
    {
        for (var i = 0; i < 20; i++)
        {
            var title = _fixture.Create<string>();
            if (string.IsNullOrWhiteSpace(title))
                continue;
            var slug = SlugNormalizer.FromTitle(title);
            Assert.False(string.IsNullOrEmpty(slug));
            Assert.DoesNotContain(' ', slug);
        }
    }
}
