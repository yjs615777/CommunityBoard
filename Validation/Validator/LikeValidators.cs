using CommunityBoard.Contracts.Requests;
using FluentValidation;

namespace CommunityBoard.Validation.Validator
{
    public class CreateLikeRequestValidator : AbstractValidator<CreateLikeRequest>
    {
        public CreateLikeRequestValidator()
        {
            RuleFor(x => x.CommentId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
    public class DeleteLikeRequestValidator : AbstractValidator<DeleteLikeRequest>
    {
        public DeleteLikeRequestValidator()
        {
            RuleFor(x => x.CommentId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }

    }
    public class ToggleLikeRequestValidator : AbstractValidator<ToggleLikeRequest>
    {
        public ToggleLikeRequestValidator()
        {
            RuleFor(x => x.CommentId).GreaterThan(0);
            RuleFor(x => x.UserId).GreaterThan(0);
        }
    }
}
