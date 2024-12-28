using FluentValidation;

public class DateRangeValidator : AbstractValidator<(DateTime StartDate, DateTime EndDate)>
{
    public DateRangeValidator()
    {
        RuleFor(range => range.StartDate)
            .NotEmpty().WithMessage("Start date is required.")
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("Start date cannot be in the past.")
            .LessThanOrEqualTo(range => range.EndDate).WithMessage("Start date cannot be after end date.");

        RuleFor(range => range.EndDate)
            .NotEmpty().WithMessage("End date is required.")
            .GreaterThanOrEqualTo(DateTime.Now.Date).WithMessage("End date cannot be in the past.");
    }
}
