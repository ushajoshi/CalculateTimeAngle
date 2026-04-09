using TimeAngle.Models;
using TimeAngle.Services;
using Xunit;

namespace TimeAngle.Tests.Services
{
    public sealed class TimeAngleServiceTests
    {
        private readonly TimeAngleService _service = new();

        [Theory]
        [InlineData("12:00", 12, 0, 0)]
        [InlineData("00:00", 0, 0, 0)]
        [InlineData("03:00", 3, 0, 90)]
        [InlineData("06:00", 6, 0, 180)]
        [InlineData("09:00", 9, 0, 90)]
        [InlineData("03:30", 3, 30, 75)]
        [InlineData("12:30", 12, 30, 165)]
        public void Calculate_WithCombinedTime_ReturnsExpectedAngle(
            string time,
            int expectedHour,
            int expectedMinutes,
            double expectedAngle)
        {
            // Arrange
            var request = new TimeAngleRequest
            {
                Time = time
            };

            // Act
            var result = _service.Calculate(request);

            // Assert
            Assert.Equal(expectedHour, result.Hour);
            Assert.Equal(expectedMinutes, result.Minutes);
            Assert.Equal(expectedAngle, result.Angle);
        }

        [Theory]
        [InlineData(12, 0, 0)]
        [InlineData(0, 0, 0)]
        [InlineData(3, 0, 90)]
        [InlineData(6, 0, 180)]
        [InlineData(9, 0, 90)]
        [InlineData(3, 30, 75)]
        [InlineData(12, 30, 165)]
        public void Calculate_WithSeparateHourAndMinutes_ReturnsExpectedAngle(
            int hour,
            int minutes,
            double expectedAngle)
        {
            // Arrange
            var request = new TimeAngleRequest
            {
                Hour = hour,
                Minutes = minutes
            };

            // Act
            var result = _service.Calculate(request);

            // Assert
            Assert.Equal(hour, result.Hour);
            Assert.Equal(minutes, result.Minutes);
            Assert.Equal(expectedAngle, result.Angle);
        }

        [Fact]
        public void Calculate_AtTwelveOClock_ReturnsZero_Not360()
        {
            // Arrange
            var request = new TimeAngleRequest
            {
                Hour = 12,
                Minutes = 0
            };

            // Act
            var result = _service.Calculate(request);

            // Assert
            Assert.Equal(0, result.Angle);
        }

        [Fact]
        public void Calculate_AtMidnight_ReturnsZero()
        {
            // Arrange
            var request = new TimeAngleRequest
            {
                Hour = 0,
                Minutes = 0
            };

            // Act
            var result = _service.Calculate(request);

            // Assert
            Assert.Equal(0, result.Angle);
        }

        [Fact]
        public void Calculate_WhenMinutesMissing_DefaultsToZero()
        {
            // Arrange
            var request = new TimeAngleRequest
            {
                Hour = 3
            };

            // Act
            var result = _service.Calculate(request);

            // Assert
            Assert.Equal(3, result.Hour);
            Assert.Equal(0, result.Minutes);
            Assert.Equal(90, result.Angle);
        }
    }
}