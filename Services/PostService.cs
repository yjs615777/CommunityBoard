using CommunityBoard.Common;
using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Entities;
using CommunityBoard.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Linq;


namespace CommunityBoard.Services
{
    public class PostService(IPostRepository posts) : IPostService
    {
        private readonly IPostRepository _posts = posts;

        public async Task<Result<PagedResult<PostListItemDto>>> GetPagedAsync(PageQuery query, int? categoryId = null, CancellationToken ct = default)
        {


            IQueryable<Post> q = _posts.Query()
                  .AsNoTracking()
                  .Include(p => p.Author)
                  .Include(p => p.Comments)
                  .OrderByDescending(p => p.IsPinned)
                  .ThenByDescending(p => p.CreatedAt);

            if (categoryId.HasValue)
            {
                var cat = categoryId.Value;        
                q = q.Where(p => p.CategoryId == cat);
            }

            var total = await q.CountAsync(ct);

            var items = await q.Skip(query.Skip)
                               .Take(query.PageSize)
                               .Select(p => new PostListItemDto(
                                   p.Id,
                                   p.Title,
                                   p.Author.Name,
                                   p.IsPinned,
                                   p.CreatedAt,
                                   p.ViewCount,
                                   p.Comments.Count,
                                   p.CategoryId
                               ))
                               .ToListAsync(ct);

            return Result<PagedResult<PostListItemDto>>.Ok(
                new PagedResult<PostListItemDto>(items, total, query.Page, query.PageSize));
        }

        public async Task<Result<PostDetailDto>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var p = await _posts.Query()
              .Include(x => x.Author)
              .FirstOrDefaultAsync(x => x.Id == id, ct);

            if (p is null) return Result<PostDetailDto>.Fail("not_found", $"Post #{id} not found.");

            p.ViewCount++;
            await _posts.UpdateAsync(p, ct);

            var dto = new PostDetailDto(
                p.Id,
                p.Title,
                p.Content,
                p.CategoryId,
                p.IsPinned,
                p.CreatedAt,
                p.UpdatedAt,
                p.ViewCount,
                p.Author.Name,
                Array.Empty<CommentDto>()
            );
            return Result<PostDetailDto>.Ok(dto);
        }

        public async Task<Result<int>> CreateAsync(CreatePostRequest req, CancellationToken ct = default)
        {
            var entity = new Post
            {
                Title = req.Title,
                Content = req.Content,
                CategoryId = req.CategoryId,
                AuthorId = req.AuthorId,
                IsPinned = false
            };

            await _posts.AddAsync(entity, ct);
            return Result<int>.Ok(entity.Id);
        }

        public async Task<Result> UpdateAsync(int id, UpdatePostRequest req, CancellationToken ct = default)
        {
            var post = await _posts.GetByIdAsync(id, ct);
            if (post is null)
                return Result.Fail("not_found", $"Post #{id} not found.");

            post.Title = req.Title;
            post.Content = req.Content;
            post.CategoryId = req.CategoryId;
            post.IsPinned = req.IsPinned;
            post.UpdatedAt = DateTime.UtcNow;

            await _posts.UpdateAsync(post, ct);
            return Result.Ok();
        }

        public async Task<Result> DeleteAsync(int id, CancellationToken ct = default)
        {
            var post = await _posts.GetByIdAsync(id, ct);
            if (post is null)
                return Result.Fail("not_found", $"Post #{id} not found.");

            await _posts.DeleteAsync(post, ct);
            return Result.Ok();
        }

        public async Task<Result> TogglePinAsync(int postId, CancellationToken ct = default)
        {
            var post = await _posts.GetByIdAsync(postId, ct);
            if (post is null)
                return Result.Fail("not_found", "게시글을 찾을 수 없습니다.");

            post.IsPinned = !post.IsPinned;
            await _posts.UpdateAsync(post, ct);
            return Result.Ok();
        }
    }
}
