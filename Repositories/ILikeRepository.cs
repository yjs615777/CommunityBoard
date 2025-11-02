using CommunityBoard.Entities;

namespace CommunityBoard.Repositories
{
    public interface ILikeRepository
    {
        IQueryable<Like> Query();
        Task<bool> ExistsAsync(int commentId, int userId, CancellationToken ct = default);
        Task<Like?> FindAsync(int commentId, int userId, CancellationToken ct = default);
        Task AddAsync(Like entity, CancellationToken ct = default);
        Task RemoveAsync(Like entity, CancellationToken ct = default);
        Task<int> CountByCommentIdAsync(int commentId, CancellationToken ct = default);
    }
}
