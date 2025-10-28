using CommunityBoard.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Entities;

namespace CommunityBoard.Services
{
    public interface ICommentService

    {
        Task<Result<int>> CreateAsync(CreateCommentRequest req, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, int authorId, CancellationToken ct = default);
        Task<Result<Comment>> GetByIdAsync(int id, CancellationToken ct = default);
    }
}
