using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Contracts.Requests;

    public record CreatePostRequest(
    string Title,
    string Content,
    int CategoryId,
    int AuthorId // JWT 붙이기 전: 요청으로 받고, 나중엔 토큰에서 추출
);

    public record UpdatePostRequest(
    string Title,
    string Content,
    int CategoryId,
    bool IsPinned
);

