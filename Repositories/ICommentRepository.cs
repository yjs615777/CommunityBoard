using CommunityBoard.Entities;

namespace CommunityBoard.Repositories
{
    public interface ICommentRepository
    {
        IQueryable<Comment> Query();
        Task<Comment?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Comment entity, CancellationToken ct = default);
        Task DeleteAsync(Comment entity, CancellationToken ct = default);
        Task SaveAsync(CancellationToken ct = default);
    }
}
