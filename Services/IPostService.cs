using CommunityBoard.Common;
using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Entities;

namespace CommunityBoard.Services
{
    public interface IPostService
    {
        Task<Result<PagedResult<PostListItemDto>>> GetPagedAsync(PageQuery query, int? categoryId = null, CancellationToken ct = default);
        Task<Result<PostDetailDto>> GetByIdAsync(int id, CancellationToken ct = default);
        Task<Result<int>> CreateAsync(CreatePostRequest req, CancellationToken ct = default);
        Task<Result> UpdateAsync(int id, UpdatePostRequest req, CancellationToken ct = default);
        Task<Result> DeleteAsync(int id, CancellationToken ct = default);
        Task<Result> TogglePinAsync(int postId, CancellationToken ct = default);
    }
}
