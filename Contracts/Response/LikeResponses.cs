namespace CommunityBoard.Contracts.Response
{
    public record LikeResponse(
    int Id,
    int CommentId,
    string CommentContent,
    int PostId,
    string PostTitle,
    DateTime CreatedAt
);
}
