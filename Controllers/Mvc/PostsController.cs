using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace CommunityBoard.Controllers.Mvc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class PostsController(IPostService service, ILogger<PostsController> logger) : Controller
    {
        private readonly IPostService _service = service;
        private readonly ILogger<PostsController> _logger = logger;

        private bool IsOwnerOrAdmin(string authorName)
        {
            return User.IsInRole("Admin") ||
                   string.Equals(User.Identity?.Name, authorName, StringComparison.OrdinalIgnoreCase);
        }

        // GET: /Posts
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index(
            int page = 1,
            int pageSize = 10,
            CancellationToken ct = default)
        {
            try
            {
                _logger.LogInformation("게시글 목록 요청: Page={Page}, Size={Size}", page, pageSize);
                // PageQuery는 Page, PageSize만 받음
                var query = new PageQuery(page, pageSize);

                var res = await _service.GetPagedAsync(query, categoryId: 1, ct);
                if (!res.Success || res.Data is null)
                {
                    _logger.LogWarning("게시글 목록 조회 실패: {Message}", res.Error?.Message);
                    TempData["Error"] = res.Error?.Message ?? "목록을 불러오지 못했습니다.";

                    // PagedResult<T>(Items, Total, Page, Size)
                    var empty = new PagedResult<PostListItemDto>(
                        Array.Empty<PostListItemDto>(), 0, page, pageSize);

                    return View(empty); // 모델: PagedResult<PostListItemDto>
                }
                _logger.LogInformation("게시글 목록 조회 성공: Count={Count}", res.Data.Items.Count);
                return View(res.Data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "게시글 목록 조회 중 예외 발생");
                throw;
            }
        }

        // GET: /Posts/Detail/5
        [HttpGet("Posts/Detail/{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Detail(int id, CancellationToken ct)
        {
            try
            {
                int? currentUserId = null;
                if (User.Identity?.IsAuthenticated == true)
                {
                    var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                    if (claim != null && int.TryParse(claim.Value, out var uid))
                        currentUserId = uid;
                }
                _logger.LogInformation("게시글 상세 요청: PostId={PostId}, UserId={UserId}", id, currentUserId);

                var res = await _service.GetByIdAsync(id, currentUserId, ct);
                if (!res.Success || res.Data is null)
                {
                    _logger.LogWarning("게시글 상세 조회 실패: PostId={PostId}, Msg={Msg}", id, res.Error?.Message);
                    TempData["Error"] = res.Error?.Message ?? "게시글을 찾을 수 없습니다.";
                    return RedirectToAction(nameof(Index));
                }
                _logger.LogInformation("게시글 상세 조회 성공: PostId={PostId}", id);
                return View(res.Data); // 모델: PostDetailDto
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "게시글 상세 조회 중 예외 발생 (PostId={PostId})", id);
                throw;
            }
        }

        // GET: /Posts/Create
        [HttpGet]
        public IActionResult Create()
        {
            _logger.LogInformation("게시글 작성 폼 접근 (User={User})", User.Identity?.Name);
            var model = new CreatePostRequest("", "", 1, 0);
            return View(model); // 모델: CreatePostRequest
        }

        // POST: /Posts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreatePostRequest req, CancellationToken ct)
        {
            try
            {
                // 로그인 사용자 ID 추출
                var claim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (claim is null || !int.TryParse(claim.Value, out var uid))
                {
                    _logger.LogWarning("게시글 작성 실패 - 로그인 정보 없음");
                    ModelState.AddModelError(string.Empty, "로그인 정보가 유효하지 않습니다.");
                    return View(req);
                }

                // 서버에서 최종 보정 (이 컨트롤러는 '자유게시판'이므로 1 고정)
                req = req with { AuthorId = uid, CategoryId = 1 };
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("게시글 작성 실패 - 유효성 검증 실패 (UserId={UserId})", uid);
                    return View(req);
                }

                _logger.LogInformation("게시글 작성 요청: UserId={UserId}, Title={Title}", uid, req.Title);
                var res = await _service.CreateAsync(req, ct);

                if (!res.Success || res.Data == 0)
                {
                    _logger.LogWarning("게시글 작성 실패: {Message}", res.Error?.Message);
                    ModelState.AddModelError(string.Empty, res.Error?.Message ?? "게시글 생성 실패");
                    return View(req);
                }

                _logger.LogInformation("게시글 작성 성공: PostId={PostId}, UserId={UserId}", res.Data, uid);
                TempData["Success"] = "게시글이 등록되었습니다.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "게시글 작성 중 예외 발생");
                throw;
            }
        }

        // GET: /Posts/Edit/5
        [HttpGet("Posts/Edit/{id:int}")]
        public async Task<IActionResult> Edit(int id, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("게시글 수정 요청: PostId={PostId}", id);
                var res = await _service.GetByIdAsync(id, null, ct);
                if (!res.Success || res.Data is null)
                {
                    _logger.LogWarning("게시글 수정 대상 없음: PostId={PostId}", id);
                    TempData["Error"] = res.Error?.Message ?? "게시글을 찾을 수 없습니다.";
                    return RedirectToAction(nameof(Index));
                }

                if (!IsOwnerOrAdmin(res.Data.AuthorName))
                {
                    _logger.LogWarning("게시글 수정 권한 없음: PostId={PostId}, User={User}", id, User.Identity?.Name);
                    return Forbid();
                }
                var p = res.Data;
                var vm = new UpdatePostRequest(
                    Title: p.Title,
                    Content: p.Content,
                    CategoryId: p.CategoryId,
                    IsPinned: p.IsPinned
                );

                ViewData["PostId"] = id;
                _logger.LogInformation("게시글 수정 폼 로드 성공: PostId={PostId}", id);
                return View(vm); // 모델: UpdatePostRequest
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "게시글 수정 폼 로드 중 예외 발생 (PostId={PostId})", id);
                throw;
            }
        }

        // POST: /Posts/Edit/5
        [HttpPost("Posts/Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] UpdatePostRequest req, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("게시글 수정 요청: PostId={PostId}", id);

                var resGet = await _service.GetByIdAsync(id, null, ct);
                if (!resGet.Success || resGet.Data is null)
                {
                    _logger.LogWarning("게시글 수정 대상 없음: PostId={PostId}", id);
                    TempData["Error"] = "게시글을 찾을 수 없습니다.";
                    return RedirectToAction(nameof(Index));
                }

                // 작성자 또는 관리자만 수정 가능
                if (!IsOwnerOrAdmin(resGet.Data.AuthorName))
                {
                    _logger.LogWarning("게시글 수정 권한 없음: PostId={PostId}, User={User}", id, User.Identity?.Name);
                    return Forbid();
                }

                if (!ModelState.IsValid)
                {
                    ViewData["PostId"] = id;
                    return View(req);
                }

                var res = await _service.UpdateAsync(id, req, ct);
                if (!res.Success)
                {
                    _logger.LogWarning("게시글 수정 실패: PostId={PostId}, Msg={Msg}", id, res.Error?.Message);
                    ModelState.AddModelError(string.Empty, res.Error?.Message ?? "수정 실패");
                    ViewData["PostId"] = id;
                    return View(req);
                }

                _logger.LogInformation("게시글 수정 성공: PostId={PostId}", id);
                TempData["Success"] = "게시글이 수정되었습니다.";
                return RedirectToAction(nameof(Detail), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "게시글 수정 중 예외 발생 (PostId={PostId})", id);
                throw;
            }
        }

        // POST: /Posts/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("게시글 삭제 요청: PostId={PostId}", id);
                var resGet = await _service.GetByIdAsync(id, null, ct);
                if (!resGet.Success || resGet.Data is null)
                {
                    _logger.LogWarning("게시글 삭제 실패 - 대상 없음: PostId={PostId}", id);
                    TempData["Error"] = "게시글을 찾을 수 없습니다.";
                    return RedirectToAction(nameof(Index));
                }

                // 작성자 또는 관리자만 삭제 가능
                if (!IsOwnerOrAdmin(resGet.Data.AuthorName))
                {
                    _logger.LogWarning("게시글 삭제 권한 없음: PostId={PostId}, User={User}", id, User.Identity?.Name);
                    return Forbid();
                }

                var res = await _service.DeleteAsync(id, ct);
                if (!res.Success)
                {
                    _logger.LogWarning("게시글 삭제 실패: {Message}", res.Error?.Message);
                    TempData["Error"] = res.Error?.Message ?? "삭제 실패";
                    return RedirectToAction(nameof(Detail), new { id });
                }

                _logger.LogInformation("게시글 삭제 성공: PostId={PostId}", id);
                TempData["Success"] = "게시글이 삭제되었습니다.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "게시글 삭제 중 예외 발생 (PostId={PostId})", id);
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
        //    if (!res.Success) TempData["Error"] = res.Error?.Message ?? "실패";
        //    else TempData["Success"] = "핀 상태가 변경되었습니다.";
        //    return RedirectToAction("Detail", new { id });
        //}
    }
}

