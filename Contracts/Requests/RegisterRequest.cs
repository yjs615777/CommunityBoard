using CommunityBoard.Validation;

namespace CommunityBoard.Contracts.Requests;


public record RegisterRequest(
    [property: RequiredEmail] string Email,
    [property: PersonName] string Name,
    [property: Password] string Password,
    string ConfirmPassword
);
