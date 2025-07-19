using Claims.Features.Covers.Models;
using FluentValidation;

namespace Claims.Features.Covers.Validators;

public class CreateCoverDtoValidator : AbstractValidator<CreateCoverDto>
{
    public CreateCoverDtoValidator()
    {
        RuleFor(x => x.StartDate)
            .Must(startDate => startDate.Date >= DateTime.Today)
            .WithMessage("Start date cannot be in the past");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("End date must be after start date");

        RuleFor(x => x)
            .Must(cover => (cover.EndDate - cover.StartDate).TotalDays <= 365)
            .WithMessage("Total insurance period cannot exceed 1 year")
            .WithName("Insurance Period");

        RuleFor(x => x.Premium)
            .GreaterThan(0)
            .WithMessage("Premium must be greater than 0");

        RuleFor(x => x.Type)
            .IsInEnum()
            .WithMessage("Invalid cover type");
    }
}