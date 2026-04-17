using BlogPlatform.Domain.Entities;

namespace BlogPlatform.Tests.Unit;

public sealed class CommentApprovalTests
{
    [Fact]
    public void Approve_sets_is_approved_true()
    {
        var comment = new Comment
        {
            PostId = 1,
            AuthorName = "a",
            Content = "c",
            CreatedAt = DateTime.UtcNow,
            IsApproved = false
        };

        comment.Approve();

        Assert.True(comment.IsApproved);
    }
}
