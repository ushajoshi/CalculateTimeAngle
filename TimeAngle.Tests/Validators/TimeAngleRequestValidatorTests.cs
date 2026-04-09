using TimeAngle.Models;
using TimeAngle.Validators;
using Xunit;

namespace TimeAngle.Tests.Validators
{
    public sealed class TimeAngleRequestValidatorTests
    {
        private readonly TimeAngleRequestValidator _validator = new();

        [Fact]
        public void Validate_WhenTimeIsValid_ShouldPass()
        {
            var request = new TimeAngleRequest { 
                Time = "03:30"
            };

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_WhenHourAndMinutesAreValid_ShouldPass()
        {
            var request = new TimeAngleRequest
            {
                Hour = 3,
                Minutes = 30
            };

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_WhenOnlyHourIsProvided_ShouldPass()
        {
            var request = new TimeAngleRequest
            {
                Hour = 3
            };

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }

        [Fact]
        public void Validate_WhenNoInputProvided_ShouldFail()
        {
            var request = new TimeAngleRequest();

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage.Contains("Provide either 'time'"));
        }

        [Theory]
        [InlineData("abc")]
        [InlineData("10")]
        [InlineData("10-30")]
        [InlineData("13:00")]
        [InlineData("10:60")]
        [InlineData("10:99")]
        [InlineData("99:10")]
        public void Validate_WhenTimeFormatIsInvalid_ShouldFail(string invalidTime)
        {
            var request = new TimeAngleRequest
            {
                Time = invalidTime
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(13)]
        public void Validate_WhenHourIsOutOfRange_ShouldFail(int invalidHour)
        {
            var request = new TimeAngleRequest
            {
                Hour = invalidHour,
                Minutes = 10
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage.Contains("Hour must be between 1 and 12"));
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(60)]
        [InlineData(100)]
        public void Validate_WhenMinutesAreOutOfRange_ShouldFail(int invalidMinutes)
        {
            var request = new TimeAngleRequest
            {
                Hour = 3,
                Minutes = invalidMinutes
            };

            var result = _validator.Validate(request);

            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e =>
                e.ErrorMessage.Contains("Minutes must be between 0 and 59"));
        }
           

        [Fact]
        public void Validate_WhenTimeIsPresent_SeparateFieldsAreIgnored()
        {
            var request = new TimeAngleRequest
            {
                Time = "03:30"
            };

            var result = _validator.Validate(request);

            Assert.True(result.IsValid);
        }
    }
}