using TimeAngle.Models;

namespace TimeAngle.Services
{
    public interface ITimeAngleService
    {
        TimeAngleResponse Calculate(TimeAngleRequest request);
    }
}