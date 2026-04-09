using TimeAngle.Models;

namespace TimeAngle.Services
{
    public sealed class TimeAngleService : ITimeAngleService
    {
        public TimeAngleResponse Calculate(TimeAngleRequest request)
        {
            var (hour, minutes) = ResolveTime(request);

            var normalizedHour = hour % 12;
            var hourHandAngle = (normalizedHour * 30) + (minutes * 0.5);
            var minuteHandAngle = minutes * 6;

            var difference = Math.Abs(hourHandAngle - minuteHandAngle);
            var smallestAngle = Math.Min(difference, 360 - difference);

            return new TimeAngleResponse
            {
                Hour = hour,
                Minutes = minutes,
                Angle = smallestAngle
            };
        }

        private static (int hour, int minutes) ResolveTime(TimeAngleRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Time))
            {
                var parts = request.Time.Split(':');
                return (int.Parse(parts[0]), int.Parse(parts[1]));
            }

            return (request.Hour ?? 0, request.Minutes ?? 0);
        }
    }
}