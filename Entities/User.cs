namespace CommunityBoard.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Role { get; set; } = "User";

        public ICollection<Post> Posts { get; set; } = new List<Post>(); // 내가 썼던 글들
        public ICollection<Comment> Comments { get; set; } = new List<Comment>(); // 내가 썼던 댓글들
        public ICollection<Like> Likes { get; set; } = new List<Like>(); // 내가 좋아요한 댓글들

    }
}
    