using CommunityBoard.Contracts.Requests;
using CommunityBoard.Entities;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityBoard.Controllers.Mvc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    [Route("[controller]/[action]")]
    public class CommentsController(ICommentService service, ILikeService likeService, ILogger<CommentsController> logger) : Controller
    {

        private readonly ICommentService _service = service;
        private readonly ILogger<CommentsController> _logger = logger;
        private readonly ILikeService _likes = likeService;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCommentRequest req, CancellationToken ct)
        {
            try
            {
                // 로그인 사용자 ID 가져오기 (AuthorId)
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("댓글 작성 실패 - 로그인 정보 없음 (PostId={PostId})", req.PostId);
                    return Forbid();
                }

                int authorId = int.Parse(userIdClaim.Value);
                req = req with { AuthorId = authorId }; // 서버에서 ID 주입

                _logger.LogInformation("댓글 생성 요청 수신: PostId={PostId}, AuthorId={AuthorId}", req.PostId, authorId);

                var res = await _service.CreateAsync(req, ct);
                if (!res.Success)
                {
                    _logger.LogWarning("댓글 생성 실패: PostId={PostId}, AuthorId={AuthorId}, Code={Code}, Message={Msg}",
                            req.PostId, authorId, res.Error?.Code, res.Error?.Message);
                    TempData["Error"] = res.Error?.Message ?? "댓글 등록 실패";
                }
                else
                {
                    _logger.LogInformation("댓글 생성 성공: CommentId={CommentId}, PostId={PostId}", res.Data, req.PostId);
                }

                return RedirectToAction("Detail", "Posts", new { id = req.PostId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "댓글 생성 중 예외 발생 (PostId={PostId})", req.PostId);
                throw;
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int postId, CancellationToken ct)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    _logger.LogWarning("댓글 삭제 실패 - 로그인 정보 없음 (CommentId={CommentId})", id);
                    return Forbid();
                }

                int requesterId = int.Parse(userIdClaim.Value);
                bool isAdmin = User.IsInRole("Admin");

                _logger.LogInformation("댓글 삭제 요청: CommentId={CommentId}, UserId={UserId}, IsAdmin={IsAdmin}", id, requesterId, isAdmin);

                var res = await _service.DeleteAsync(id, requesterId, isAdmin, ct);
                if (!res.Success)
                {
                    _logger.LogWarning("댓글 삭제 실패: CommentId={CommentId}, Code={Code}, Msg={Msg}",
                            id, res.Error?.Code, res.Error?.Message);
                    TempData["Error"] = res.Error?.Message ?? "댓글 삭제 실패";
                }
                else
                {
                    _logger.LogInformation("댓글 삭제 성공: CommentId={CommentId}, PostId={PostId}", id, postId);
                }

                return RedirectToAction("Detail", "Posts", new { id = postId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "댓글 삭제 중 예외 발생 (CommentId={CommentId})", id);
                throw;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLikeAjax(int id, CancellationToken ct)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim is null)
                {
                    _logger.LogWarning("좋아요 토글 실패 - 로그인 정보 없음 (CommentId={CommentId})", id);
                    return Forbid();
                }

                if (!int.TryParse(userIdClaim.Value, out var userId))
                {
                    _logger.LogWarning("좋아요 토글 실패 - 잘못된 사용자 ID (CommentId={CommentId}, Value={Value})",
                            id, userIdClaim.Value);
                    return BadRequest(new { error = "Invalid user id" });
                }
                _logger.LogInformation("좋아요 토글 요청: CommentId={CommentId}, UserId={UserId}", id, userId);

                var res = await _likes.ToggleAsync(id, userId, ct);
                if (!res.Success)
                {
                    _logger.LogWarning("좋아요 토글 실패: CommentId={CommentId}, Code={Code}, Msg={Msg}",
                       id, res.Error?.Code, res.Error?.Message);
                    return BadRequest(new { error = res.Error?.Message ?? "toggle failed" });
                }

                _logger.LogInformation("좋아요 토글 성공: CommentId={CommentId}, Count={Count}, Liked={Liked}",
                   id, res.Data.count, res.Data.liked);

                return Json(new { count = res.Data.count, liked = res.Data.liked });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "좋아요 토글 중 예외 발생 (CommentId={CommentId})", id);
                throw;
            }
        }


    }
}