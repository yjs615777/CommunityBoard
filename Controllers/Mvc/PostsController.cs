using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CommunityBoard.Controllers.Mvc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class PostsController(IPostService service,ILogger<PostsController> logger) : Controller
    {
        private readonly IPostService _service = service;
        private readonly ILogger<PostsController> _logger = logger;

        // GET: /Posts
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default)
        {
            // PageQuery는 Page, PageSize만 받음
            var query = new PageQuery(page, pageSize);

            var res = await _service.GetPagedAsync(query, categoryId: 1, ct);
            if (!res.Success || res.Data is null)
            {
                TempData["Error"] = res.Error?.Message ?? "목록을 불러오지 못했습니다.";

                // PagedResult<T>(Items, Total, Page, Size)
                var empty = new PagedResult<PostListItemDto>(
                    Array.Empty<PostListItemDto>(), 0, page, pageSize);

                return View(empty); // 모델: PagedResult<PostListItemDto>
            }

            return View(res.Data);
        }

        // GET: /Posts/Detail/5
        [HttpGet("Posts/Detail/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id, CancellationToken ct)
        {
            var res = await _service.GetByIdAsync(id, ct);
            if (!res.Success || res.Data is null)
            {
                TempData["Error"] = res.Error?.Message ?? "게시글을 찾을 수 없습니다.";
                return RedirectToAction(nameof(Index));
            }

            return View(res.Data); // 모델: PostDetailDto
        }

        // GET: /Posts/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(); // 모델: CreatePostRequest
        }

        // POST: /Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid)
                return View(req);

            var res = await _service.CreateAsync(req, ct);
            if (!res.Success || res.Data == 0)
            {
                ModelState.AddModelError(string.Empty, res.Error?.Message ?? "게시글 생성 실패");
                return View(req);
            }

            TempData["Success"] = "게시글이 등록되었습니다.";
            return RedirectToAction(nameof(Detail), new { id = res.Data });
        }

        // GET: /Posts/Edit/5
        [HttpGet("Posts/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            var res = await _service.GetByIdAsync(id, ct);
            if (!res.Success || res.Data is null)
            {
                TempData["Error"] = res.Error?.Message ?? "게시글을 찾을 수 없습니다.";
                return RedirectToAction(nameof(Index));
            }

            var p = res.Data;
            var vm = new UpdatePostRequest(
                Title: p.Title,
                Content: p.Content,
                CategoryId: p.CategoryId,
                IsPinned: p.IsPinned
            );

            ViewData["PostId"] = id;
            return View(vm); // 모델: UpdatePostRequest
        }

        // POST: /Posts/Edit/5
        [HttpPost("Posts/Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdatePostRequest req, CancellationToken ct)
        {
            if (!ModelState.IsValid)
            {
                ViewData["PostId"] = id;
                return View(req);
            }

            var res = await _service.UpdateAsync(id, req, ct);
            if (!res.Success)
            {
                ModelState.AddModelError(string.Empty, res.Error?.Message ?? "수정 실패");
                ViewData["PostId"] = id;
                return View(req);
            }

            TempData["Success"] = "게시글이 수정되었습니다.";
            return RedirectToAction(nameof(Detail), new { id });
        }

        // POST: /Posts/Delete/5
        [HttpPost("Posts/Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var res = await _service.DeleteAsync(id, ct);
            if (!res.Success)
            {
                TempData["Error"] = res.Error?.Message ?? "삭제 실패";
                return RedirectToAction(nameof(Detail), new { id });
            }

            TempData["Success"] = "게시글이 삭제되었습니다.";
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TogglePin(int id, CancellationToken ct)
        {
            var res = await _service.TogglePinAsync(id, ct);
            if (!res.Success) TempData["Error"] = res.Error?.Message ?? "실패";
            else TempData["Success"] = "핀 상태가 변경되었습니다.";
            return RedirectToAction("Detail", new { id });
        }
    }
}

