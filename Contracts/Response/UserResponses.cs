namespace CommunityBoard.Contracts.Response
{
    public record UserResponse(
      int Id,
      string Name,
      string Email,
      string Role
  );

}
