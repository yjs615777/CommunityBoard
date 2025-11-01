using CommunityBoard.Contracts.Requests;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace CommunityBoard.Controllers.Mvc
{
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    public class CommentsController(ICommentService service, ILogger<CommentsController> logger) : Controller
    {
        private readonly ICommentService _service = service;
        private readonly ILogger<CommentsController> _logger = logger;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] CreateCommentRequest req, CancellationToken ct)
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
    }
}