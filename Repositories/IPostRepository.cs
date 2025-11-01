using CommunityBoard.Entities;

namespace CommunityBoard.Repositories
{
    public interface IPostRepository
    {
        IQueryable<Post> Query();
        Task<Post?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(Post entity, CancellationToken ct = default);
        Task UpdateAsync(Post entity, CancellationToken ct = default);
        Task DeleteAsync(Post entity, CancellationToken ct = default);
        Task SaveAsync(CancellationToken ct = default);
        Task<Post?> GetWithCommentsByIdAsync(int id, CancellationToken ct = default);
    }
}
