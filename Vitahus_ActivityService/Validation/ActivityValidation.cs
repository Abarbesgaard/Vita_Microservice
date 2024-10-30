using FluentValidation;
using Vitahus_ActivityService_Shared;

namespace Vitahus_ActivityService.Validation
{
    public class ActivityValidator : AbstractValidator<Activity>
    {
        public ActivityValidator()
        {
			const int maxTitleLength = 100;
			const int minTitleLength = 1;
			const int maxDescriptionLength = 500;
			const int minDescriptionLength = 1;
			const string TitleRequiredMessage = "Title is required";

            RuleFor(activity => activity.UserId)
              .NotEmpty()
              .WithMessage("UserId is required");
            RuleFor(activity => activity.Title)
              .NotEmpty()
              .WithMessage(TitleRequiredMessage)
              .Length(minTitleLength, maxTitleLength)
              .WithMessage("Title should be between 1 and 100 characters");
            RuleFor(activity => activity.Description)
              .NotEmpty()
              .WithMessage("Description is required")
              .Length(minDescriptionLength, maxDescriptionLength)
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
