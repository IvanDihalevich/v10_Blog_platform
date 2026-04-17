using BlogPlatform.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Api.Services;

public class SlugService(BlogDbContext db) : ISlugService
{
    public async Task<string> EnsureUniqueSlugAsync(string baseSlug, int? excludePostId, CancellationToken cancellationToken = default)
    {
        var slug = baseSlug;
        var n = 2;
        while (await db.Posts.AnyAsync(
                   p => p.Slug == slug && (!excludePostId.HasValue || p.Id != excludePostId.Value),
                   cancellationToken))
        {
            slug = $"{baseSlug}-{n}";
            n++;
        }

        return slug;
    }
}
