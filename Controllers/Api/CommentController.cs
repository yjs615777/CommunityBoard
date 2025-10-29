using AutoMapper;
using CommunityBoard.Common;
using CommunityBoard.Contracts.Requests;
using CommunityBoard.Contracts.Response;
using CommunityBoard.Services;
using Microsoft.AspNetCore.Mvc;

namespace CommunityBoard.Controllers.Api
{
    [ApiController]
    [Route("api/controller")]
    public class CommentController(ICommentService service , IMapper mapper) : ControllerBase
    {
        private readonly ICommentService _service = service;
        private readonly IMapper _mapper = mapper;
        /// <summary>댓글 생성</summary>
        [HttpPost]
        [ProducesResponseType(typeof(Result<int>), 201)]
        [ProducesResponseType(typeof(Result<object>), 400)]
        public async Task<ActionResult<Result<int>>> Create([FromBody] CreateCommentRequest req, CancellationToken ct)
        {
            var res = await _service.CreateAsync(req, ct);
            if (!res.Success) return BadRequest(res);

            return StatusCode(201, Result<int>.Ok(res.Data));
        }

        /// <summary>댓글 단건 조회</summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(Result<CommentDto>), 200)]
        [ProducesResponseType(typeof(Result<object>), 404)]
        public async Task<ActionResult<Result<CommentDto>>> GetById(int id, CancellationToken ct)
        {
            var res = await _service.GetByIdAsync(id, ct);
            if (!res.Success) return NotFound(res);

            var dto = _mapper.Map<CommentDto>(res.Data!);
            return Ok(Result<CommentDto>.Ok(dto));
        }

        /// <summary>댓글 삭제 (작성자만)</summary>
        [HttpDelete("{id:int}")]
        [ProducesResponseType(typeof(Result), 200)]
        [ProducesResponseType(typeof(Result<object>), 403)]
        [ProducesResponseType(typeof(Result<object>), 404)]
        public async Task<ActionResult<Result>> Delete(int id, [FromQuery] int authorId, CancellationToken ct)
        {
            var res = await _service.DeleteAsync(id, authorId, ct);
            if (!res.Success)
            {
                return res.Error?.Code switch
                {
                    "not_found" => NotFound(res),
                    "forbidden" => StatusCode(403, res),
                    _ => BadRequest(res)
                };
            }
            return Ok(res);
        }
    }
}
