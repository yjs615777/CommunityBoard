using CommunityBoard.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Entities;
using CommunityBoard.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CommunityBoard.Services
{
    public class CommentService(ICommentRepository comments) : ICommentService
    {
        private readonly ICommentRepository _comments = comments;

        public async Task<Result<int>> CreateAsync(CreateCommentRequest req, CancellationToken ct = default)
        {
            var entity = new Comment
            {
                PostId = req.PostId,
                AuthorId = req.AuthorId,
                Content = req.Content,
                // CreatedAt은 OnModelCreating에서 GETUTCDATE()로 기본값 설정됨
            };

            await _comments.AddAsync(entity, ct);
            return Result<int>.Ok(entity.Id);
        }

        public async Task<Result> DeleteAsync(int id, int requesterUserId, bool isAdmin, CancellationToken ct = default)
        {
            var entity = await _comments.GetByIdAsync(id, ct);
            if (entity is null)
                return Result.Fail("not_found", $"Comment #{id} not found.");

            if (!isAdmin && entity.AuthorId != requesterUserId)
                return Result.Fail("forbidden", "삭제 권한이 없습니다.");

            await _comments.DeleteAsync(entity, ct);
            return Result.Ok();
        }

        public async Task<Result<Comment>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var c = await _comments.Query()                // ICommentRepository.Query()
                .AsNoTracking()
                .Include(x => x.Author)                    // ← Author 로드
                .Include(x => x.Likes)                     // ← Likes 로드
                .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (c is null)
                return Result<Comment>.Fail("not_found", $"Comment #{id} not found.");

            return Result<Comment>.Ok(c);
        }
     
    }
}
