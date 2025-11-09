using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Contracts.Requests;

public record CreateCommentRequest
{
    public int PostId { get; init; }  // init → 생성 시 1회만 설정 가능 (불변처럼)
    public int AuthorId { get; init; } // 클라이언트 입력 무시됨, 컨트롤러에서 JWT Claims로 설정
    public string Content { get; set; } = null!; // set 가능 (사용자 입력) 
}

