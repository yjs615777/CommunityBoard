using CommunityBoard.Contracts.Requests;
using FluentValidation;

namespace CommunityBoard.Validation.Validator
{
    public class CreatePostRequestValidator : AbstractValidator<CreatePostRequest>
    {
        public CreatePostRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("제목은 필수입니다.")
                .MaximumLength(200);
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("본문은 필수입니다.")
                .MinimumLength(3);
            RuleFor(x => x.CategoryId)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.AuthorId)
                .GreaterThan(0);
        }
    }
}
