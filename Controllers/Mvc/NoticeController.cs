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
    public class NoticeController(IPostService service, ILogger<NoticeController> logger) : Controller
    {
        private readonly IPostService _service = service;
        private readonly ILogger<NoticeController> _logger = logger;

        private const int NoticeCategoryId = 3; // 공지 카테고리 (원하면 변경)

        // GET: /Notice
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default)
        {
            var query = new PageQuery(page, pageSize);

            // 우선 공지 카테고리만 가져온 뒤
            var res = await _service.GetPagedAsync(query, categoryId: NoticeCategoryId, ct);
            if (!res.Success || res.Data is null)
            {
                TempData["Error"] = res.Error?.Message ?? "공지 목록을 불러오지 못했습니다.";
                var empty = new PagedResult<PostListItemDto>(Array.Empty<PostListItemDto>(), 0, page, pageSize);
                return View(empty);
            }
            return View(res.Data);

        }

        // GET: /Notice/Create (관리자만)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {   
            // 기본값: 공지 카테고리 + 고정글
            return View(new CreatePostRequest("", "", NoticeCategoryId, 0));
        }

        // POST: /Notice/Create (관리자만)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest req, CancellationToken ct)
        {
            // 로그인한 관리자 ID 추출
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null || !int.TryParse(claim.Value, out var uid))
            {
                ModelState.AddModelError(string.Empty, "로그인 정보가 유효하지 않습니다.");
                return View(req);
            }

            // 서버에서 최종 강제: 공지 카테고리 + 작성자
            req = req with { CategoryId = NoticeCategoryId, AuthorId = uid };


            if (!ModelState.IsValid)
                return View(req);

            var res = await _service.CreateAsync(req, ct);
            if (!res.Success || res.Data == 0)
            {
                ModelState.AddModelError(string.Empty, res.Error?.Message ?? "공지 등록 실패");
                return View(req);
            }

            // 생성 후 고정 상태 보장을 위해(혹시 기본값이 false일 수 있으니) TogglePin 호출해도 되고,
            // 더 깔끔하게 하려면 PostService.CreateAsync 내에서 IsPinned = true로 저장하도록 확장해도 됨.
            TempData["Success"] = "공지사항이 등록되었습니다.";
            return RedirectToAction(nameof(Index));
        }
    }
}