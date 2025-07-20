using Claims.Features.Claims.Models;
using FluentValidation;

namespace Claims.Features.Claims.Validators;

public class CreateClaimDtoValidator : AbstractValidator<CreateClaimDto>
{
    public CreateClaimDtoValidator()
    {
        RuleFor(x => x.DamageCost)
            .GreaterThan(0)
            .WithMessage("Damage cost must be greater than 0")
            .LessThanOrEqualTo(100000)
            .WithMessage("Damage cost cannot exceed 100,000");

        RuleFor(x => x.CoverId)
            .NotEmpty()
            .WithMessage("Cover ID is required");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Claim name is required")
            .MaximumLength(100)
            .WithMessage("Claim name cannot exceed 100 characters");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid claim type");
    }
}