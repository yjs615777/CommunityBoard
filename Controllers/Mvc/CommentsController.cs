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
    public class CommentsController(ICommentService service,ILikeService likeService, ILogger<CommentsController> logger) : Controller
    {

        private readonly ICommentService _service = service;
        private readonly ILogger<CommentsController> _logger = logger;
        private readonly ILikeService _likes = likeService;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create( CreateCommentRequest req, CancellationToken ct)
        {
            // 로그인 사용자 ID 가져오기 (AuthorId)
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
                return Forbid();

            int authorId = int.Parse(userIdClaim.Value);

            req = req with { AuthorId = authorId }; // 서버에서 ID 주입

            var res = await _service.CreateAsync(req, ct);
            if (!res.Success)
            {
                TempData["Error"] = res.Error?.Message ?? "댓글 등록 실패";
            }

            return RedirectToAction("Detail", "Posts", new { id = req.PostId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int postId, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Forbid();

            int requesterId = int.Parse(userIdClaim.Value);
            bool isAdmin = User.IsInRole("Admin");

            var res = await _service.DeleteAsync(id, requesterId, isAdmin, ct);
            if (!res.Success)
                TempData["Error"] = res.Error?.Message ?? "댓글 삭제 실패";

            return RedirectToAction("Detail", "Posts", new { id = postId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleLikeAjax(int id, CancellationToken ct)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim is null) return Forbid();

            if (!int.TryParse(userIdClaim.Value, out var userId))
                return BadRequest(new { error = "Invalid user id" });

            var res = await _likes.ToggleAsync(id, userId, ct);
            if (!res.Success)
                return BadRequest(new { error = res.Error?.Message ?? "toggle failed" });

            return Json(new { count = res.Data.count, liked = res.Data.liked });
        }

    }
}