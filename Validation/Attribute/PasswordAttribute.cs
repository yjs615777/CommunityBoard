using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Validation.Attribute
{
    public sealed class PasswordAttribute : StringLengthAttribute
    {
        public PasswordAttribute(int min = 6, int max = 100) : base(max)
        {
            MinimumLength = min;
            ErrorMessage = "비밀번호는 6~100자 사이여야 합니다.";
        }
    }
}
