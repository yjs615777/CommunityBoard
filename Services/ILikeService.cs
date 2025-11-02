using CommunityBoard.Common;

namespace CommunityBoard.Services
{
    public interface ILikeService
    {
        Task<Result> AddAsync(int commentId, int userId, CancellationToken ct = default);
        Task<Result> RemoveAsync(int commentId, int userId, CancellationToken ct = default);

        Task<Result<(int count, bool liked)>> ToggleAsync(int commentId, int userId, CancellationToken ct = default);
    }
}
