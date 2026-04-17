using BlogPlatform.Api.Models;
using BlogPlatform.Api.Services;
using BlogPlatform.Domain.Entities;
using BlogPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostsController(BlogDbContext db, ISlugService slugService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedPostsDto>> GetPublished(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? tag = null,
        CancellationToken cancellationToken = default)
    {
        if (page < 1) page = 1;
        if (pageSize is < 1 or > 100) pageSize = 20;

        var query = db.Posts
            .AsNoTracking()
            .Where(p => p.IsPublished)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(tag))
        {
            var t = tag.Trim();
            query = query.Where(p => p.PostTags.Any(pt => pt.Tag.Name == t));
        }

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.PublishedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new PostListItemDto(
                p.Id,
                p.Title,
                p.Slug,
                p.AuthorName,
                p.PublishedAt,
                p.ViewCount,
                p.PostTags.Select(pt => pt.Tag.Name).OrderBy(n => n).ToList()))
            .ToListAsync(cancellationToken);

        return Ok(new PagedPostsDto(items, total, page, pageSize));
    }

    [HttpGet("{slug}")]
    public async Task<ActionResult<PostDetailDto>> GetBySlug(string slug, CancellationToken cancellationToken)
    {
        var affected = await db.Posts
            .Where(p => p.Slug == slug && p.IsPublished)
            .ExecuteUpdateAsync(
                s => s.SetProperty(p => p.ViewCount, p => p.ViewCount + 1),
                cancellationToken);

        if (affected == 0)
            return NotFound();

        var post = await db.Posts
            .AsNoTracking()
            .Include(p => p.PostTags)
            .ThenInclude(pt => pt.Tag)
            .Where(p => p.Slug == slug && p.IsPublished)
            .Select(p => new PostDetailDto(
                p.Id,
                p.Title,
                p.Content,
                p.AuthorName,
                p.Slug,
                p.PublishedAt,
                p.IsPublished,
                p.ViewCount,
                p.PostTags.Select(pt => pt.Tag.Name).OrderBy(n => n).ToList()))
            .FirstOrDefaultAsync(cancellationToken);

        return post is null ? NotFound() : Ok(post);
    }

    [HttpPost]
    public async Task<ActionResult<object>> CreateDraft([FromBody] CreatePostRequest request, CancellationToken cancellationToken)
    {
        var baseSlug = SlugNormalizer.FromTitle(request.Title);
        var slug = await slugService.EnsureUniqueSlugAsync(baseSlug, null, cancellationToken);

        var post = new Post
        {
            Title = request.Title,
            Content = request.Content ?? string.Empty,
            AuthorName = request.AuthorName,
            Slug = slug,
            PublishedAt = null,
            IsPublished = false,
            ViewCount = 0
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetBySlug), new { slug = post.Slug }, new { post.Id, post.Slug });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePostRequest request, CancellationToken cancellationToken)
    {
        var post = await db.Posts.FindAsync([id], cancellationToken);
        if (post is null)
            return NotFound();

        post.Title = request.Title;
        post.Content = request.Content ?? string.Empty;
        post.AuthorName = request.AuthorName;

        var baseSlug = SlugNormalizer.FromTitle(request.Title);
        post.Slug = await slugService.EnsureUniqueSlugAsync(baseSlug, id, cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPatch("{id:int}/publish")]
    public async Task<IActionResult> Publish(int id, CancellationToken cancellationToken)
    {
        var post = await db.Posts.FindAsync([id], cancellationToken);
        if (post is null)
            return NotFound();

        if (string.IsNullOrWhiteSpace(post.Content))
            return BadRequest(new { error = "Cannot publish a post with empty content." });

        post.IsPublished = true;
        post.PublishedAt ??= DateTime.UtcNow;

        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var post = await db.Posts.FindAsync([id], cancellationToken);
        if (post is null)
            return NotFound();

        db.Posts.Remove(post);
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:int}/comments")]
    public async Task<ActionResult<CommentDto>> AddComment(int id, [FromBody] CreateCommentRequest request, CancellationToken cancellationToken)
    {
        var postExists = await db.Posts.AnyAsync(p => p.Id == id, cancellationToken);
        if (!postExists)
            return NotFound();

        var comment = new Comment
        {
            PostId = id,
            AuthorName = request.AuthorName,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            IsApproved = false
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(
            nameof(GetComments),
            new { id },
            new CommentDto(comment.Id, comment.PostId, comment.AuthorName, comment.Content, comment.CreatedAt, comment.IsApproved));
    }

    [HttpGet("{id:int}/comments")]
    public async Task<ActionResult<IReadOnlyList<CommentDto>>> GetComments(int id, CancellationToken cancellationToken)
    {
        var postExists = await db.Posts.AnyAsync(p => p.Id == id, cancellationToken);
        if (!postExists)
            return NotFound();

        var list = await db.Comments
            .AsNoTracking()
            .Where(c => c.PostId == id && c.IsApproved)
            .OrderBy(c => c.CreatedAt)
            .Select(c => new CommentDto(c.Id, c.PostId, c.AuthorName, c.Content, c.CreatedAt, c.IsApproved))
            .ToListAsync(cancellationToken);

        return Ok(list);
    }
}
