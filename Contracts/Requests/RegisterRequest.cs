
using CommunityBoard.Validation.Attribute;
using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Contracts.Requests;


public class RegisterRequest
{
    [RequiredEmail]
    public string Email { get; set; } = "";

    [PersonName]
    public string Name { get; set; } = "";

    [Password]
    public string Password { get; set; } = "";

    [Compare("Password", ErrorMessage = "비밀번호가 일치하지 않습니다.")]
    public string ConfirmPassword { get; set; } = "";
}
