using CommunityBoard.Data;
using CommunityBoard.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Repositories
{
    public class LikeRepository : ILikeRepository
    {
        private readonly CommunityContext _db;
        public LikeRepository(CommunityContext db) => _db = db;

        public IQueryable<Like> Query()
            => _db.Likes.AsNoTracking()
                        .Include(l => l.User)
                        .Include(l => l.Comment).ThenInclude(c => c.Post);

        public Task<bool> ExistsAsync(int commentId, int userId, CancellationToken ct = default)
            => _db.Likes.AnyAsync(l => l.CommentId == commentId && l.UserId == userId, ct);

        public Task<Like?> FindAsync(int commentId, int userId, CancellationToken ct = default)
            => _db.Likes.FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId, ct);

        public async Task AddAsync(Like entity, CancellationToken ct = default)
        {
            _db.Likes.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task RemoveAsync(Like entity, CancellationToken ct = default)
        {
            _db.Likes.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }
    }
}
