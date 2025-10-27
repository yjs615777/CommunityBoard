using CommunityBoard.Entities;

namespace CommunityBoard.Contracts.Response
{
    public record PostListItemDto(
    int Id,
    string Title,
    string AuthorName,
    bool IsPinned,
    DateTime CreatedAt,
    int ViewCount,
    int CommentCount
);

    public record PostDetailDto(
        int Id,
        string Title,
        string Content,
        int CategoryId,
        bool IsPinned,
        DateTime CreatedAt,
        DateTime UpdatedAt,
        int ViewCount,
        string AuthorName,
        IReadOnlyList<CommentDto> Comments
    );
}
