namespace CommunityBoard.Contracts.Requests;

    // 댓글에 좋아요 추가
    public record CreateLikeRequest(int CommentId, int UserId); // JWT 전 임시

    // 댓글 좋아요 취소
    public record DeleteLikeRequest(int CommentId, int UserId);

    // (선택) 토글 방식이 필요하면 이 한 개로 대체 가능
    public record ToggleLikeRequest(int CommentId, int UserId);

