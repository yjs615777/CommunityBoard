namespace CommunityBoard.Contracts.Response
{
    public record CommentDto(
       int Id,
       string AuthorName,
       string Content,
       DateTime CreatedAt,
       int LikeCount,
       bool LikedByCurrentUser // JWT 붙인 뒤 실제 값 계산(지금은 false 고정 가능)
   );
}
