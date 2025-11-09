using System.ComponentModel.DataAnnotations;

namespace CommunityBoard.Validation.Attribute
{
    public class PersonNameAttribute : StringLengthAttribute
    {
        public PersonNameAttribute(int max = 30) : base(max)
        {
            MinimumLength = 2;
            ErrorMessage = "이름은 2~30자 사이여야 합니다.";
        }
    }
}
