using CommunityBoard.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Data

{
    public class CommunityContext(DbContextOptions<CommunityContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<Like> Likes => Set<Like>();
        protected override void OnModelCreating(ModelBuilder m)
        {
            // User
            m.Entity<User>(e =>
            {
                e.Property(x => x.Name).HasMaxLength(50).IsRequired();
                e.Property(x => x.Email).HasMaxLength(200).IsRequired();
                e.HasIndex(x => x.Email).IsUnique();
            });

            // Post
            m.Entity<Post>(e =>
            {
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.ViewCount).HasDefaultValue(0);

                e.HasOne(x => x.Author)
                    .WithMany(u => u.Posts)
                    .HasForeignKey(x => x.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Comment
            m.Entity<Comment>(e =>
            {
                e.Property(x => x.Content).HasColumnType("nvarchar(max)").IsRequired();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.HasOne(x => x.Post)
                    .WithMany()
                    .HasForeignKey(x => x.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.Author)
                    .WithMany(u => u.Comments)
                    .HasForeignKey(x => x.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Like
            m.Entity<Like>(e =>
            {
                e.HasOne(x => x.Comment)
                    .WithMany(c => c.Likes)
                    .HasForeignKey(x => x.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(x => x.User)
                    .WithMany(u => u.Likes)
                    .HasForeignKey(x => x.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasIndex(x => new { x.CommentId, x.UserId }).IsUnique();
            });
        }
    }

}
