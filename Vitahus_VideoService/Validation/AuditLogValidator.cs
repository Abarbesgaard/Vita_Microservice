using FluentValidation;
using Vitahus_VideoService_Shared;
namespace Vitahus_VideoService.Validation
{

    public class AuditLogValidator : AbstractValidator<AuditLog>
    {
        public AuditLogValidator()
        {
            RuleFor(auditLog => auditLog.UserId)
              .NotEmpty()
              .WithMessage("UserId is required");

            RuleFor(auditLog => auditLog.Operation)
              .NotEmpty()
              .WithMessage("Operation is required")
              .Length(1, 100)
              .WithMessage("Operation should be between 1 and 100 characters");

            RuleFor(auditLog => auditLog.Collection)
              .NotEmpty()
              .WithMessage("Collection is required")
              .Length(1, 100)
              .WithMessage("Collection should be between 1 and 100 characters");

            RuleFor(auditLog => auditLog.DocumentId)
              .NotEmpty()
              .WithMessage("DocumentId is required");
        }

    }
}

