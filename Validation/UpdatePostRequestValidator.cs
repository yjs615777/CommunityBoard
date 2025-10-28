using CommunityBoard.Contracts.Requests;
using FluentValidation;

namespace CommunityBoard.Validation
{
    public class UpdatePostRequestValidator : AbstractValidator<UpdatePostRequest>
    {
        public UpdatePostRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("제목은 필수입니다.")
                .MaximumLength(200);
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("본문은 필수입니다.")
                .MinimumLength(3);
            RuleFor(x => x.CategoryId)
                .GreaterThanOrEqualTo(0);
            // IsPinned는 bool이라 기본 통과
        }
    }
}
