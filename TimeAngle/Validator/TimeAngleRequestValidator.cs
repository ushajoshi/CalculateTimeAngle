using FluentValidation;
using TimeAngle.Models;

namespace TimeAngle.Validators
{
    public sealed class TimeAngleRequestValidator : AbstractValidator<TimeAngleRequest>
    {
        public TimeAngleRequestValidator()
        {
            // Rule: Must provide either time OR hour
            RuleFor(x => x)
                .Must(HaveValidInput)
                .WithMessage("Provide either 'time' in HH:MM format OR 'hour' with optional 'minutes'.");

            // Validate combined time (HH:MM)
            When(x => !string.IsNullOrWhiteSpace(x.Time), () =>
            {
                RuleFor(x => x.Time!)
                    .Must(BeValidTimeFormat)
                    .WithMessage("Time must be in HH:MM format with valid values (1-12 hours, 0-59 minutes).");
            });

            // Validate separate params
            When(x => string.IsNullOrWhiteSpace(x.Time), () =>
            {
                RuleFor(x => x.Hour)
                    .NotNull().WithMessage("Hour is required.")
                    .InclusiveBetween(1, 12).WithMessage("Hour must be between 1 and 12.");

                RuleFor(x => x.Minutes)
                    .InclusiveBetween(0, 59)
                    .When(x => x.Minutes.HasValue)
                    .WithMessage("Minutes must be between 0 and 59.");
            });

            RuleFor(x => x)
            .Custom((x, context) =>
            {
                bool hasTime = !string.IsNullOrWhiteSpace(x.Time);
                bool hasHourMinute = x.Hour.HasValue && x.Minutes.HasValue;

                if (hasTime && hasHourMinute)
                {
                    context.AddFailure("Either Time OR Hour and Minutes should be provided, not both.");
                }

             
                if (hasTime && x.Hour.HasValue && !x.Minutes.HasValue)
                {
                    context.AddFailure("Either Time OR Hour and Minutes should be provided, not both.");
                }

                if (hasTime && !x.Hour.HasValue && x.Minutes.HasValue)
                {
                    context.AddFailure("Either Time OR Hour and Minutes should be provided, not both.");
                }
            });
        }

        private static bool HaveValidInput(TimeAngleRequest request)
        {
            return !string.IsNullOrWhiteSpace(request.Time) || request.Hour.HasValue;
        }

        private static bool BeValidTimeFormat(string time)
        {
            var parts = time.Split(':');
            if (parts.Length != 2) return false;

            if (!int.TryParse(parts[0], out int hour)) return false;
            if (!int.TryParse(parts[1], out int minutes)) return false;

            return hour >= 1 && hour <= 12 && minutes >= 0 && minutes <= 59;
        }
    }
}