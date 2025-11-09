using CommunityBoard.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Data;

public static class SeedData
{
    public static async Task InitializeAsync(IServiceProvider services, ILogger logger, CancellationToken ct = default)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;
        var env = sp.GetRequiredService<IWebHostEnvironment>(); // IHostEnvironment도 OK
        var cfg = sp.GetRequiredService<IConfiguration>();
        var db = sp.GetRequiredService<CommunityContext>();

        // 1) 마이그레이션: 환경변수로 온/오프 (기본 ON)
        var runMigrations = cfg.GetValue("RUN_MIGRATIONS", true);
        if (runMigrations)
        {
            logger.LogInformation("Applying EF Core migrations…");
            await db.Database.MigrateAsync(ct);
        }

        // 환경 변수에서 관리자 이메일과 비밀번호를 읽어서 계정을 생성
        var adminEmail = cfg["ADMIN_EMAIL"];      
        var adminPass = cfg["ADMIN_PASSWORD"];   
        if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPass))
        {
            if (!await db.Users.AnyAsync(u => u.Email == adminEmail, ct))
            {
                var admin = new User
                {
                    Name = "Admin",
                    Email = adminEmail,
                    Role = "Admin"
                };
                var hasher = new PasswordHasher<User>();
                admin.PasswordHash = hasher.HashPassword(admin, adminPass);

                db.Users.Add(admin);
                await db.SaveChangesAsync(ct);

                logger.LogInformation("Seeded Admin user: {Email}", adminEmail);
            }
        }
        else
        {
            logger.LogWarning("Admin seeding skipped. ADMIN_EMAIL/ADMIN_PASSWORD not provided.");
        }


        // 3) 개발 전용 더미 데이터 (샘플 유저/게시글)
        // !!이 블록은 Development 환경에서만 동작함 (서버 배포 시 자동 비활성화)
        var seedDemo = cfg.GetValue("SEED_DEMO", env.EnvironmentName == "Development");
        if (seedDemo)
        {
            if (!await db.Users.AnyAsync(ct))
            {
                var u1 = new User { Name = "Jinsoo", Email = "jinsoo@example.com" };
                var u2 = new User { Name = "Alice", Email = "alice@example.com" };

                // 데모 비번은 'dev'로 해시
                var hasher = new PasswordHasher<User>();
                u1.PasswordHash = hasher.HashPassword(u1, "dev");
                u2.PasswordHash = hasher.HashPassword(u2, "dev");

                db.Users.AddRange(u1, u2);
                await db.SaveChangesAsync(ct);

                db.Posts.AddRange(
                    new Post { Title = "첫 글", Content = "안녕하세요!", AuthorId = u1.Id, CategoryId = 1, IsPinned = true },
                    new Post { Title = "두 번째 글", Content = "EF Core 연결 완료", AuthorId = u2.Id, CategoryId = 1 }
                );
                await db.SaveChangesAsync(ct);

                logger.LogInformation("개발 환경용 샘플 사용자와 게시글이 시드되었습니다");
            }
        }
        else
        {
            logger.LogInformation("이 실행 환경에서는 데모 시드 기능이 비활성화되었습니다");
        }
    }
}
