namespace TimeAngle.Models
{
    public sealed class TimeAngleRequest
    {
        /// <summary>
        /// Combined time in HH:MM format. Example: 03:30
        /// </summary>
        public string? Time { get; init; }

        /// <summary>
        /// Hour component when using separate parameters. Valid range: 0-12.
        /// </summary>
        public int? Hour { get; init; }

        /// <summary>
        /// Minute component when using separate parameters. Valid range: 0-59.
        /// </summary>
        public int? Minutes { get; init; }
    }
}