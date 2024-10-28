using FluentValidation;
using Vitahus_VideoService_Shared;
namespace Vitahus_VideoService.Validation
{
    public class BaseEntityValidator : AbstractValidator<BaseEntity>
    {
        public BaseEntityValidator()
        {
            RuleFor(baseEntity => baseEntity.Id)
                .NotEmpty()
                .WithMessage("Id is required")
                .NotEqual(System.Guid.Empty)
                .WithErrorCode("Id must not be empty");

            RuleFor(baseEntity => baseEntity.UserId)
                .NotEmpty()
                .WithMessage("UserId is required")
                .NotEqual(System.Guid.Empty)
                .WithErrorCode("UserId must not be empty");

            RuleFor(baseEntity => baseEntity.Title)
              .NotEmpty()
              .WithMessage("Title is required")
              .Length(1, 200)
              .WithMessage("Title should be between 1 and 200 characters");

            RuleFor(baseEntity => baseEntity.Description)
              .NotEmpty()
              .WithMessage("Description is required")
              .Length(1, 500)
              .WithMessage("Description should be between 1 and 500 characters");

            RuleFor(baseEntity => baseEntity.CreatedAt)
              .NotEmpty()
              .WithMessage("CreatedAt is required and must have a date")
              .LessThanOrEqualTo(System.DateTime.Now)
              .WithMessage("CreatedAt must be less than or equal to the current date");

            RuleFor(baseEntity => baseEntity.CreatedBy)
              .NotEmpty()
              .WithMessage("CreatedBy is required")
              .Length(1, 200)
              .WithMessage("CreatedBy should be between 1 and 200 characters");

            RuleFor(baseEntity => baseEntity.UpdatedBy)
              .NotEmpty()
              .WithMessage("UpdatedBy is required")
              .Length(1, 200)
              .WithMessage("UpdatedBy should be between 1 and 200 characters");
        }
    }
}
