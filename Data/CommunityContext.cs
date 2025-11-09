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

                // User → Post 관계 (유저 삭제 시 글은 남기기)
                e.HasMany(u => u.Posts)
                    .WithOne(p => p.Author)
                    .HasForeignKey(p => p.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User → Comment 관계 (유저 삭제 시 댓글은 남기기)
                e.HasMany(u => u.Comments)
                    .WithOne(c => c.Author)
                    .HasForeignKey(c => c.AuthorId)
                    .OnDelete(DeleteBehavior.Restrict);

                // User → Like 관계 (유저 삭제 시 좋아요 삭제)
                e.HasMany(u => u.Likes)
                    .WithOne(l => l.User)
                    .HasForeignKey(l => l.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Post
            m.Entity<Post>(e =>
            {
                e.Property(x => x.Title).HasMaxLength(200).IsRequired();
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.UpdatedAt).HasDefaultValueSql("GETUTCDATE()");
                e.Property(x => x.ViewCount).HasDefaultValue(0);

                e.HasMany(p => p.Comments)
                    .WithOne(c => c.Post)
                    .HasForeignKey(c => c.PostId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Comment
            m.Entity<Comment>(e =>
            {
                e.Property(x => x.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Comment → Like (댓글 삭제 시 좋아요 삭제)
                e.HasMany(c => c.Likes)
                    .WithOne(l => l.Comment)
                    .HasForeignKey(l => l.CommentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Like
            m.Entity<Like>(e =>
            {
                // 하나의 댓글에 한 유저가 중복으로 좋아요 누르지 못하게 제약 설정
                e.HasIndex(x => new { x.CommentId, x.UserId }).IsUnique();
            });

        }
    }

}
