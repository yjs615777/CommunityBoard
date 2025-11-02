using CommunityBoard.Data;
using CommunityBoard.Entities;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Repositories
{
    public class PostRepository : IPostRepository
    {
        private readonly CommunityContext _db;
        public PostRepository(CommunityContext db) => _db = db;

        // 목록/검색용 기본 쿼리: 읽기 최적화(AsNoTracking) + 필수 Include만
        public IQueryable<Post> Query()
            => _db.Posts.AsQueryable();

        public Task<Post?> GetByIdAsync(int id, CancellationToken ct = default)
            => _db.Posts
                  .Include(p => p.Author)
                  .FirstOrDefaultAsync(p => p.Id == id, ct);

        public async Task AddAsync(Post entity, CancellationToken ct = default)
        {
            _db.Posts.Add(entity);
            await _db.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Post entity, CancellationToken ct = default)
        {
            // 트래킹 상태가 Detached일 수 있으니 Attach 후 수정 표시
            _db.Attach(entity);
            _db.Entry(entity).State = EntityState.Modified;
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Post entity, CancellationToken ct = default)
        {
            _db.Posts.Remove(entity);
            await _db.SaveChangesAsync(ct);
        }

        public Task SaveAsync(CancellationToken ct = default)
            => _db.SaveChangesAsync(ct);

        public async Task<Post?> GetWithCommentsByIdAsync(int id, CancellationToken ct = default)
        {
            return await _db.Posts
                .Include(p => p.Author)
                .Include(p => p.Comments).ThenInclude(c => c.Author)
                .Include(p => p.Comments).ThenInclude(c => c.Likes)
                .AsSplitQuery()
                .FirstOrDefaultAsync(p => p.Id == id, ct);
        }
    }
}
