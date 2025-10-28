using CommunityBoard.Common;
using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Entities;
using CommunityBoard.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CommunityBoard.Services
{
    public class PostService(IPostRepository posts) : IPostService
    {
        private readonly IPostRepository _posts = posts;

        public async Task<Result<PagedResult<Post>>> GetPagedAsync(PageQuery query, CancellationToken ct = default)
        {
            var q = _posts.Query()
                          .OrderByDescending(p => p.CreatedAt);

            var total = await q.CountAsync(ct);

            var items = await q.Skip(query.Skip)
                               .Take(query.PageSize)
                               .ToListAsync(ct);

            var page = new PagedResult<Post>(items, total, query.Page, query.PageSize);
            return Result<PagedResult<Post>>.Ok(page);
        }

        public async Task<Result<Post>> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var post = await _posts.GetByIdAsync(id, ct);
            if (post is null)
                return Result<Post>.Fail("not_found", $"Post #{id} not found.");
            return Result<Post>.Ok(post);
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

    }
}
