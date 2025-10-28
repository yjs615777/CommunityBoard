using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Contracts.Requests
{
    public record CreateCommentRequest(
        int PostId,
        int AuthorId,   // JWT 전 임시  
        string Content
    );
}
