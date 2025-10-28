using CommunityBoard.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Entities;
using CommunityBoard.Repositories;

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

        public async Task<Result> DeleteAsync(int id, int authorId, CancellationToken ct = default)
        {
            var entity = await _comments.GetByIdAsync(id, ct);
            if (entity is null)
                return Result.Fail("not_found", $"Comment #{id} not found.");

            // 자신의 댓글만 삭제 가능 (인증 붙기 전 임시 규칙)
            if (entity.AuthorId != authorId)
                return Result.Fail("forbidden", "본인 댓글만 삭제할 수 있습니다.");

            await _comments.DeleteAsync(entity, ct);
            return Result.Ok();
        }

        public async Task<Result<Comment>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _comments.GetByIdAsync(id, ct);
            if (entity is null)
                return Result<Comment>.Fail("not_found", $"Comment #{id} not found.");
            return Result<Comment>.Ok(entity);
        }
    }
}
