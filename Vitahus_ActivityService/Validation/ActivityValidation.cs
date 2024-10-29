using FluentValidation;
using Vitahus_ActivityService_Shared;

namespace Vitahus_ActivityService.Validation
{
    public class ActivityValidator : AbstractValidator<Activity>
    {
        public ActivityValidator()
        {
            RuleFor(activity => activity.UserId)
              .NotEmpty()
              .WithMessage("UserId is required");
            RuleFor(activity => activity.Title)
              .NotEmpty()
              .WithMessage("Title is required")
              .Length(1, 100)
              .WithMessage("Title should be between 1 and 100 characters");
            RuleFor(activity => activity.Description)
              .NotEmpty()
              .WithMessage("Description is required")
              .Length(1, 100)
              .WithMessage("Description should be between 1 and 100 characters");
            RuleFor(activity => activity.CreatedBy)
              .NotEmpty()
              .WithMessage("CreatedBy is required")
              .Length(1, 100)
              .WithMessage("CreatedBy should be between 1 and 100 characters");
            RuleFor(activity => activity.UpdatedBy)
              .NotEmpty()
              .WithMessage("UpdatedBy is required")
              .Length(1, 100)
              .WithMessage("UpdatedBy should be between 1 and 100 characters");
        }
    }
}
