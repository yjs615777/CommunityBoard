using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Validation
{
    public sealed class RequiredEmailAttribute : ValidationAttribute
    {
        private readonly EmailAddressAttribute _email = new();
        public RequiredEmailAttribute()
        {
            ErrorMessage = "올바른 이메일 주소를 입력하세요.";
        }

        public override bool IsValid(object? value)
        {
            var s = value as string;
            return !string.IsNullOrWhiteSpace(s) && _email.IsValid(s);
        }
    }
}
