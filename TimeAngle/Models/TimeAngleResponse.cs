namespace TimeAngle.Models
{
    public sealed class TimeAngleResponse
    {
        public int Hour { get; init; }
        public int Minutes { get; init; }
        public double Angle { get; init; }
    }
}