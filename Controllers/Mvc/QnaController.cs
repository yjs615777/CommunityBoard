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
    public class QnaController(IPostService service, ILogger<QnaController> logger) : Controller
    {
        private readonly IPostService _service = service;
        private readonly ILogger<QnaController> _logger = logger;

        // GET: /Qna
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default)
        {
            var query = new PageQuery(page, pageSize);
            var res = await _service.GetPagedAsync(query, categoryId: 2, ct);
            if (!res.Success || res.Data is null)
            {
                TempData["Error"] = res.Error?.Message ?? "목록을 불러오지 못했습니다.";
                var empty = new PagedResult<PostListItemDto>(
                    Array.Empty<PostListItemDto>(), 0, page, pageSize);
                return View(empty);
            }
            return View(res.Data); // 모델: PagedResult<PostListItemDto>
        }

        // GET: /Qna/Create
        [HttpGet]
        public IActionResult Create() => View(new CreatePostRequest("", "", 2, 0));

        // POST: /Qna/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest req, CancellationToken ct)
        {
            // 서버에서 한 번 더 보정/검증: 문의게시판은 무조건 2
            req = req with { CategoryId = 2 };

            if (!ModelState.IsValid) return View(req);

            var res = await _service.CreateAsync(req, ct);
            if (!res.Success || res.Data == 0)
            {
                ModelState.AddModelError(string.Empty, res.Error?.Message ?? "등록 실패");
                return View(req);
            }
            TempData["Success"] = "문의글이 등록되었습니다.";
            return RedirectToAction(nameof(Index));
        }
    }
}