using FluentValidation;
using UserActivity.Application.DTOs;

namespace UserActivity.Application.Validators;
public class DateRangeValidator : AbstractValidator<DateRangeDto>
{
    public DateRangeValidator()
    {
        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .LessThan(x => x.EndDate).WithMessage("Start date must be earlier than end date.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("End date must be later than or equal to start date.");
    }
}

