namespace BlogPlatform.Api.Models;

public record PostListItemDto(
    int Id,
    string Title,
    string Slug,
    string AuthorName,
    DateTime? PublishedAt,
    int ViewCount,
    IReadOnlyList<string> Tags);

public record PostDetailDto(
    int Id,
    string Title,
    string Content,
    string AuthorName,
    string Slug,
    DateTime? PublishedAt,
    bool IsPublished,
    int ViewCount,
    IReadOnlyList<string> Tags);

public record PagedPostsDto(
    IReadOnlyList<PostListItemDto> Items,
    int TotalCount,
    int Page,
    int PageSize);

public record CreatePostRequest(string Title, string Content, string AuthorName);

public record UpdatePostRequest(string Title, string Content, string AuthorName);

public record CreateCommentRequest(string AuthorName, string Content);

public record CommentDto(int Id, int PostId, string AuthorName, string Content, DateTime CreatedAt, bool IsApproved);
