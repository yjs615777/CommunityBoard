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
            try
            {
                _logger.LogInformation("문의게시판 목록 요청: Page={Page}, Size={Size}", page, pageSize);

                var query = new PageQuery(page, pageSize);
                var res = await _service.GetPagedAsync(query, categoryId: 2, ct);

                if (!res.Success || res.Data is null)
                {
                    _logger.LogWarning("문의게시판 목록 불러오기 실패: {Message}", res.Error?.Message);
                    TempData["Error"] = res.Error?.Message ?? "목록을 불러오지 못했습니다.";

                    var empty = new PagedResult<PostListItemDto>(
                        Array.Empty<PostListItemDto>(), 0, page, pageSize);
                    return View(empty);
                }

                _logger.LogInformation("문의게시판 목록 로드 성공: Count={Count}", res.Data.Items.Count);
                return View(res.Data); // 모델: PagedResult<PostListItemDto>
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "문의게시판 목록 조회 중 예외 발생");
                throw;
            }
        }

        // GET: /Qna/Create
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("문의글 작성 폼 접근: User={User}", User.Identity?.Name);
            var model = new CreatePostRequest("", "", 2, 0);
            return View(model);
        }

        // POST: /Qna/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest req, CancellationToken ct)
        {
            try
            {
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim is null || !int.TryParse(claim.Value, out var uid))
                {
                    _logger.LogWarning("문의글 작성 실패 - 로그인 정보 없음");
                    ModelState.AddModelError(string.Empty, "로그인 정보가 유효하지 않습니다.");
                    return View(req);
                }

                req = req with { CategoryId = 2, AuthorId = uid };

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("문의글 작성 실패 - 유효성 검증 실패 (UserId={UserId})", uid);
                    return View(req);
                }

                _logger.LogInformation("문의글 작성 요청: UserId={UserId}, Title={Title}", uid, req.Title);
                var res = await _service.CreateAsync(req, ct);

                if (!res.Success || res.Data == 0)
                {
                    _logger.LogWarning("문의글 작성 실패: {Message}", res.Error?.Message);
                    ModelState.AddModelError(string.Empty, res.Error?.Message ?? "등록 실패");
                    return View(req);
                }
                _logger.LogInformation("문의글 작성 성공: PostId={PostId}, UserId={UserId}", res.Data, uid);
                TempData["Success"] = "문의글이 등록되었습니다.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "문의글 작성 중 예외 발생");
                throw;
            }
        }

        // 추후 기능추가
        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> TogglePin(int id, CancellationToken ct)
        //{
        //    var res = await _service.TogglePinAsync(id, ct);
        //    TempData[res.Success ? "Success" : "Error"] =
        //        res.Success ? "핀 상태가 변경되었습니다." : (res.Error?.Message ?? "실패");
        //    return RedirectToAction("Detail", new { id });
        //}

    }
}