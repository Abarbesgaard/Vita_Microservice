using Vitahus_VideoService_Shared;
using FluentValidation;
namespace Vitahus_VideoService.Validation
{
    public class VideoValidator : AbstractValidator<Video>
    {
        public VideoValidator()
        {
            Include(new BaseEntityValidator());

            RuleFor(video => video.Description)
              .NotEmpty()
              .WithMessage("Description is required")
              .Length(1, 1000)
              .WithMessage("Description should be between 1 and 1000 characters");

            RuleFor(video => video.Url)
              .NotEmpty()
              .WithMessage("Url is required")
              .Length(1, 2083)
              .WithMessage("Url should be between 1 and 2083 characters")
              .Must(url => url.StartsWith("https://"))
              .WithMessage("Url must start with https://");

            RuleFor(video => video.CreatedAt)
              .NotEmpty()
              .WithMessage("CreatedAt is required and must have a date")
              .LessThanOrEqualTo(System.DateTime.Now)
              .WithMessage("CreatedAt must be less than or equal to the current date");


        }
    }
}
