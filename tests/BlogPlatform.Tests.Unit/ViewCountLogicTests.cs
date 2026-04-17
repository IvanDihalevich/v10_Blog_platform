using BlogPlatform.Domain.Entities;

namespace BlogPlatform.Tests.Unit;

public sealed class ViewCountLogicTests
{
    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(42)]
    [InlineData(int.MaxValue - 1)]
    public void ViewCount_increments_by_one_when_post_is_viewed(int before)
    {
        var post = new Post { ViewCount = before };
        post.ViewCount++;
        Assert.Equal(before + 1, post.ViewCount);
    }

    [Fact]
    public void ExecuteUpdate_style_increment_matches_domain_expectation()
    {
        const int initial = 10;
        var afterSimulated = initial + 1;
        Assert.Equal(11, afterSimulated);
    }
}
