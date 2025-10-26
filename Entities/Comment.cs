namespace CommunityBoard.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public Post Post { get; set; } = null!;
        public User Author { get; set; } = null!;
        public int PostId { get; set; }
        public int AuthorId { get; set; }
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public ICollection<Like> Likes { get; set; } = new List<Like>();


    }
}
