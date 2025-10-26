namespace CommunityBoard.Entities
{
    public class Like
    {
        public int Id { get; set; }
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public Comment Comment { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
