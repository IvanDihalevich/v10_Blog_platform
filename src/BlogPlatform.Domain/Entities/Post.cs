namespace BlogPlatform.Domain.Entities;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public bool IsPublished { get; set; }
    public int ViewCount { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();
}
