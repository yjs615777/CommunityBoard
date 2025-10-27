using CommunityBoard.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Data;

public static class SeedData
{
    public static async Task InitializeAsync(CommunityContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct); // 운영/로컬 공통: 스키마 보장

        if (!await db.Users.AnyAsync(ct))
        {
            var u1 = new User { Name = "Jinsoo", Email = "jinsoo@example.com", PasswordHash = "dev" };
            var u2 = new User { Name = "Alice", Email = "alice@example.com", PasswordHash = "dev" };
            db.Users.AddRange(u1, u2);
            await db.SaveChangesAsync(ct);

            db.Posts.AddRange(
                new Post { Title = "첫 글", Content = "안녕하세요!", AuthorId = u1.Id, CategoryId = 1, IsPinned = true },
                new Post { Title = "두 번째 글", Content = "EF Core 연결 완료", AuthorId = u2.Id, CategoryId = 1 }
            );
            await db.SaveChangesAsync(ct);
        }
    }
}