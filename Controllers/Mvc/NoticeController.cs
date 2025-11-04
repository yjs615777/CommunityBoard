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

        private const int NoticeCategoryId = 3; // 공지 카테고리 

        // GET: /Notice
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("공지 목록 조회 요청: Page={Page}, Size={Size}", page, pageSize);
                var query = new PageQuery(page, pageSize);

                // 우선 공지 카테고리만 가져온 뒤
                var res = await _service.GetPagedAsync(query, categoryId: NoticeCategoryId, ct);
                if (!res.Success || res.Data is null)
                {
                    _logger.LogWarning("공지 목록 조회 실패: Code={Code}, Message={Message}",
                        res.Error?.Code, res.Error?.Message);

                    TempData["Error"] = res.Error?.Message ?? "공지 목록을 불러오지 못했습니다.";
                    var empty = new PagedResult<PostListItemDto>(Array.Empty<PostListItemDto>(), 0, page, pageSize);
                    return View(empty);
                }
                _logger.LogInformation("공지 목록 조회 성공: Count={Count}", res.Data.Items.Count);
                return View(res.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "공지 목록 조회 중 예외 발생");
                throw;
            }
        }

        // GET: /Notice/Create (관리자만)
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            _logger.LogInformation("공지 작성 폼 접근 (User={User})", User.Identity?.Name);
            // 기본값: 공지 카테고리 + 고정글
            return View(new CreatePostRequest("", "", NoticeCategoryId, 0));
        }

        // POST: /Notice/Create (관리자만)
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest req, CancellationToken ct)
        {
            try
            {
                // 로그인한 관리자 ID 추출
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim is null || !int.TryParse(claim.Value, out var uid))
                {
                    _logger.LogWarning("공지 작성 실패 - 로그인 정보 없음");
                    ModelState.AddModelError(string.Empty, "로그인 정보가 유효하지 않습니다.");
                    return View(req);
                }

                // 서버에서 최종 강제: 공지 카테고리 + 작성자
                req = req with { CategoryId = NoticeCategoryId, AuthorId = uid };


                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("공지 작성 실패 - 유효성 검증 실패 (UserId={UserId})", uid);
                    return View(req);
                }

                _logger.LogInformation("공지 작성 요청: UserId={UserId}, Title={Title}", uid, req.Title);

                var res = await _service.CreateAsync(req, ct);
                if (!res.Success || res.Data == 0)
                {
                    _logger.LogWarning("공지 작성 실패: Code={Code}, Message={Msg}", res.Error?.Code, res.Error?.Message);
                    ModelState.AddModelError(string.Empty, res.Error?.Message ?? "공지 등록 실패");
                    return View(req);
                }


                _logger.LogInformation("공지 작성 성공: PostId={PostId}, UserId={UserId}", res.Data, uid);
                TempData["Success"] = "공지사항이 등록되었습니다.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "공지 작성 중 예외 발생");
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