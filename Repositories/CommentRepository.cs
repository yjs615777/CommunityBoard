using CommunityBoard.Data;
using CommunityBoard.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Repositories
{
    public class CommentRepository : ICommentRepository
    {
        private readonly CommunityContext _db;
        public CommentRepository(CommunityContext db) => _db = db;

        public IQueryable<Comment> Query()
            => _db.Comments.AsNoTracking()
                           .Include(c => c.Author)
                           .Include(c => c.Likes);

        public Task<Comment?> GetByIdAsync(int id, CancellationToken ct = default)
            => _db.Comments
                  .Include(c => c.Author)
                  .Include(c => c.Likes)
                  .FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task AddAsync(Comment entity, CancellationToken ct = default)
        {
            _db.Comments.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Comment entity, CancellationToken ct = default)
        {
            _db.Comments.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public Task SaveAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);
    }
}
