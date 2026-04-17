namespace BlogPlatform.Domain.Entities;

public class Comment
{
    public int Id { get; set; }
    public int PostId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsApproved { get; set; }

    public Post Post { get; set; } = null!;

    public void Approve() => IsApproved = true;
}
