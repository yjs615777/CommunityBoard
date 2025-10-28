using CommunityBoard.Common;
using CommunityBoard.Entities;
using CommunityBoard.Repositories;

namespace CommunityBoard.Services
{
    public class LikeService(ILikeRepository likes) : ILikeService
    {
        private readonly ILikeRepository _likes = likes;

        public async Task<Result> AddAsync(int commentId, int userId, CancellationToken ct = default)
        {
            var exists = await _likes.ExistsAsync(commentId, userId, ct);
            if (exists)
                return Result.Ok(); // 이미 좋아요면 OK

            var like = new Like { CommentId = commentId, UserId = userId };
            await _likes.AddAsync(like, ct);
            return Result.Ok();
        }

        public async Task<Result> RemoveAsync(int commentId, int userId, CancellationToken ct = default)
        {
            var like = await _likes.FindAsync(commentId, userId, ct);
            if (like is null)
                return Result.Ok(); // 이미 없으면 OK

            await _likes.RemoveAsync(like, ct);
            return Result.Ok();
        }
    }
}
