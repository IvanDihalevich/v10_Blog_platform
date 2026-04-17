using BlogPlatform.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlogPlatform.Infrastructure.Data;

public class BlogDbContext : DbContext
{
    public BlogDbContext(DbContextOptions<BlogDbContext> options)
        : base(options)
    {
    }

    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<PostTag> PostTags => Set<PostTag>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Title).HasMaxLength(500).IsRequired();
            e.Property(p => p.Content).IsRequired();
            e.Property(p => p.AuthorName).HasMaxLength(200).IsRequired();
            e.Property(p => p.Slug).HasMaxLength(500).IsRequired();
            e.HasIndex(p => p.Slug).IsUnique();
            e.HasIndex(p => p.IsPublished);
        });

        modelBuilder.Entity<Comment>(e =>
        {
            e.HasKey(c => c.Id);
            e.Property(c => c.AuthorName).HasMaxLength(200).IsRequired();
            e.Property(c => c.Content).IsRequired();
            e.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasIndex(c => new { c.PostId, c.IsApproved });
        });

        modelBuilder.Entity<Tag>(e =>
        {
            e.HasKey(t => t.Id);
            e.Property(t => t.Name).HasMaxLength(100).IsRequired();
            e.HasIndex(t => t.Name).IsUnique();
        });

        modelBuilder.Entity<PostTag>(e =>
        {
            e.HasKey(pt => new { pt.PostId, pt.TagId });
            e.HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
