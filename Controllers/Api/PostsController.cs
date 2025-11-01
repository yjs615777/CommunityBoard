using AutoMapper;
using CommunityBoard.Common;
using CommunityBoard.Contracts.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CommunityBoard.Controllers.Api;

[ApiController]
[Route("api/[controller]")]
public class PostsController(IPostService service, IMapper mapper) : ControllerBase
{
    private readonly IPostService _service = service;
    private readonly IMapper _mapper = mapper;
    /// <summary>게시글 목록(페이지네이션)</summary>
    [HttpGet]
    [ProducesResponseType(typeof(Result<PagedResult<PostListItemDto>>), 200)]
    public async Task<ActionResult<Result<PagedResult<PostListItemDto>>>> GetPaged([FromQuery] PageQuery query, CancellationToken ct)
    {
        var res = await _service.GetPagedAsync(query,null, ct);
        if (!res.Success) return BadRequest(res);
        return Ok(res);
    }

    /// <summary>게시글 상세</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Result<PostDetailDto>), 200)]
    [ProducesResponseType(typeof(Result<object>), 404)]
    public async Task<ActionResult<Result<PostDetailDto>>> GetById(int id, CancellationToken ct)
    {
        var res = await _service.GetByIdAsync(id, ct);
        if (!res.Success) return NotFound(res);
        return Ok(res);
    }

    /// <summary>게시글 작성</summary>
    [HttpPost]
    [ProducesResponseType(typeof(Result<int>), 201)]
    [ProducesResponseType(typeof(Result<object>), 400)]
    public async Task<ActionResult<Result<int>>> Create([FromBody] CreatePostRequest req, CancellationToken ct)
    {
        var res = await _service.CreateAsync(req, ct);
        if (!res.Success) return BadRequest(res);

        return CreatedAtAction(nameof(GetById), new { id = res.Data }, Result<int>.Ok(res.Data!));
    }

    /// <summary>게시글 수정</summary>
    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(Result<object>), 404)]
    public async Task<ActionResult<Result>> Update(int id, [FromBody] UpdatePostRequest req, CancellationToken ct)
    {
        var res = await _service.UpdateAsync(id, req, ct);
        if (!res.Success)
        {
            return res.Error?.Code == "not_found" ? NotFound(res) : BadRequest(res);
        }
        return Ok(res);
    }

    /// <summary>게시글 삭제</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(Result), 200)]
    [ProducesResponseType(typeof(Result<object>), 404)]
    public async Task<ActionResult<Result>> Delete(int id, CancellationToken ct)
    {
        var res = await _service.DeleteAsync(id, ct);
        if (!res.Success)
        {
            return res.Error?.Code == "not_found" ? NotFound(res) : BadRequest(res);
        }
        return Ok(res);
    }
}

