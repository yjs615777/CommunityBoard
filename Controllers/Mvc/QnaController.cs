using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        public IActionResult Create()
        {
            var model = new CreatePostRequest("", "", 2, 0);
            return View(model);
        }

        // POST: /Qna/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest req, CancellationToken ct)
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null || !int.TryParse(claim.Value, out var uid))
            {
                ModelState.AddModelError(string.Empty, "로그인 정보가 유효하지 않습니다.");
                return View(req);
            }

            req = req with { CategoryId = 2, AuthorId = uid };

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