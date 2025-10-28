using CommunityBoard.Common;
using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Entities;

namespace CommunityBoard.Services
{
    public interface IPostService
    {
        Task<Result<PagedResult<Post>>> GetPagedAsync(PageQuery query, CancellationToken ct = default);
        Task<Result<Post>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<int>> CreateAsync(CreatePostRequest req, CancellationToken ct = default);
        Task<Result> UpdateAsync(int id, UpdatePostRequest req, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
    }
}
