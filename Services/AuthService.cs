using CommunityBoard.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Data;
using CommunityBoard.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Services;

    public class AuthService : IAuthService
    {
        private readonly CommunityContext _db;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthService(CommunityContext db)
        {
            _db = db;
        }

        public async Task<Result<int>> RegisterAsync(RegisterRequest req, CancellationToken ct = default)
        {
            // 1) 이메일 중복 검사 (대소문자 무시 권장)
            var email = req.Email.Trim();
            var normalized = email.ToLowerInvariant();

            var exists = await _db.Users
                .AnyAsync(u => u.Email.ToLower() == normalized, ct);

            if (exists)
                return Result<int>.Fail("email_exists", "이미 사용 중인 이메일입니다.");

            // 2) User 엔티티 생성 + 패스워드 해싱
            var user = new User
            {
                Name = req.Name.Trim(),
                Email = email,
                Role = "User"
            };
            user.PasswordHash = _hasher.HashPassword(user, req.Password);

            // 3) 저장
            _db.Users.Add(user);
            await _db.SaveChangesAsync(ct);

            return Result<int>.Ok(user.Id);
        }

        public async Task<Result<User>> ValidateUserAsync(LoginRequest req, CancellationToken ct = default)
        {
            var email = req.Email.Trim();
            var normalized = email.ToLowerInvariant();

            var user = await _db.Users
                .FirstOrDefaultAsync(u => u.Email.ToLower() == normalized, ct);

            if (user is null)
                return Result<User>.Fail("invalid_login", "이메일 또는 비밀번호가 올바르지 않습니다.");

            var verify = _hasher.VerifyHashedPassword(user, user.PasswordHash, req.Password);
            if (verify == PasswordVerificationResult.Failed)
                return Result<User>.Fail("invalid_login", "이메일 또는 비밀번호가 올바르지 않습니다.");

            return Result<User>.Ok(user);
        }
    }


