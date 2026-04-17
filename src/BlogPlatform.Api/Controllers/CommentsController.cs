using BlogPlatform.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentsController(BlogDbContext db) : ControllerBase
{
    [HttpPatch("{id:int}/approve")]
    public async Task<IActionResult> Approve(int id, CancellationToken cancellationToken)
    {
        var comment = await db.Comments.FindAsync([id], cancellationToken);
        if (comment is null)
            return NotFound();

        comment.Approve();
        await db.SaveChangesAsync(cancellationToken);
        return NoContent();
    }
}
