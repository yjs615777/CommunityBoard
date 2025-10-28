namespace CommunityBoard.Contracts.Common
{
    public record PageQuery(int Page = 1, int PageSize = 20)
    {
        public int Skip => (Page - 1) * PageSize;
    }

}
