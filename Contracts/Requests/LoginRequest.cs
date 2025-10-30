using CommunityBoard.Validation;

namespace CommunityBoard.Contracts.Requests;

    public record LoginRequest(
        [property: RequiredEmail] string Email,
        [property: Password] string Password
        );

