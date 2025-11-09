using CommunityBoard.Contracts.Requests;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace CommunityBoard.Validation.Validator
{
    public class CreateCommentRequestValidator : AbstractValidator<CreateCommentRequest>
    {
        public CreateCommentRequestValidator()
        {
            RuleFor(x => x.PostId).GreaterThan(0);
            RuleFor(x => x.AuthorId).GreaterThan(0);
            RuleFor(x => x.Content)
                .NotEmpty().WithMessage("댓글 내용은 필수입니다.")
                .MaximumLength(2000);
        }
    }
}
