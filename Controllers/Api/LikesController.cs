using AutoMapper;
using CommunityBoard.Common;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CommunityBoard.Controllers.Api;
[ApiController]
[Route("api/[controller]")]
public class LikesController(ILikeService service) : ControllerBase
{
    private readonly ILikeService _service = service;

    /// <summary>좋아요 추가 (멱등)</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(Result<object>), 400)]
    public async Task<ActionResult<Result>> Add([FromQuery] int commentId, [FromQuery] int userId, CancellationToken ct)
    {
        var res = await _service.AddAsync(commentId, userId, ct);
        return res.Success ? Ok(res) : BadRequest(res);
    }

    /// <summary>좋아요 취소 (멱등)</summary>
    [HttpDelete]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(Result<object>), 400)]
    public async Task<ActionResult<Result>> Remove([FromQuery] int commentId, [FromQuery] int userId, CancellationToken ct)
    {
        var res = await _service.RemoveAsync(commentId, userId, ct);
        return res.Success ? Ok(res) : BadRequest(res);
    }

}

