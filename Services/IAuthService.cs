using CommunityBoard.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Entities;

namespace CommunityBoard.Services;

    public interface IAuthService
    {
    /// <summary>회원가입. 성공 시 새로 생성된 User Id 반환</summary>
    Task<Result<int>> RegisterAsync(RegisterRequest req, CancellationToken ct = default);

    /// <summary>로그인 자격 검증. 성공 시 User 엔티티 반환</summary>
    Task<Result<User>> ValidateUserAsync(LoginRequest req, CancellationToken ct = default);
}

