using CommunityBoard.Validation;

namespace CommunityBoard.Contracts.Requests;

public class LoginRequest
{
    [RequiredEmail]
    public string Email { get; set; } = null!;

    [Password]
    public string Password { get; set; } = null!;

    public bool RememberMe { get; set; } = false;
}

