using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Contracts.Requests
{
    public record CreateCommentRequest(
        [Required] int PostId,
        [Required] int AuthorId,   // JWT 전 임시
        [Required] string Content
    );
}
