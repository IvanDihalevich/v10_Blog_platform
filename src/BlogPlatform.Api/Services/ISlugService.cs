namespace BlogPlatform.Api.Services;

public interface ISlugService
{
    Task<string> EnsureUniqueSlugAsync(string baseSlug, int? excludePostId, CancellationToken cancellationToken = default);
}
