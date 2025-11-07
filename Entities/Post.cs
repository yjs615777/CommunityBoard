namespace CommunityBoard.Entities
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Content { get; set; } = "";
        public int AuthorId { get; set; } //User.Id를 참조하는 외래키 (FK)
        public User Author { get; set; } = null!;
        public int CategoryId { get; set; } // 게시판 분류(공지, 자유게시판, 문의 게시판)
        public bool IsPinned { get; set; }  //상단 고정 여부 (관리자만 가능)
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int ViewCount { get; set; }
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();




    }
}
