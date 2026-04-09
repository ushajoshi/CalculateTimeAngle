using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Timers;
using TimeAngle.Controllers;
using TimeAngle.Models;
using TimeAngle.Services;
using Xunit;

namespace TimeAngle.Tests.Controllers
{
    public sealed class TimeAngleApiControllerTests
    {
        private readonly Mock<ITimeAngleService> _serviceMock = new();
        private readonly Mock<IValidator<TimeAngleRequest>> _validatorMock = new();

        [Fact]
        public async Task CalculateTimeAngle_WhenRequestIsValid_ReturnsOk()
        {
            // Arrange
            var response = new TimeAngleResponse
            {
                Hour = 3,
                Minutes = 30,
                Angle = 75
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TimeAngleRequest>(), default))
                .ReturnsAsync(new FluentValidation.Results.ValidationResult());
                
            _serviceMock
                .Setup(s => s.Calculate(It.IsAny<TimeAngleRequest>()))
                .Returns(response);

            var controller = new TimeAngleApiController(
                _serviceMock.Object,
                _validatorMock.Object);

            // Act
            var result = await controller.CalculateTimeAngle("03:30", null, null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<TimeAngleResponse>(okResult.Value);

            Assert.Equal(3, value.Hour);
            Assert.Equal(30, value.Minutes);
            Assert.Equal(75, value.Angle);
        }

        [Fact]
        public async Task CalculateTimeAngle_WhenValidationFails_ReturnsBadRequest()
        {
            // Arrange
            var failures = new List<ValidationFailure>
            {
                new("Minutes", "Minutes must be between 0 and 59.")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TimeAngleRequest>(), default))
                .ReturnsAsync(new ValidationResult(failures));

            var controller = new TimeAngleApiController(
                _serviceMock.Object,
                _validatorMock.Object);

            // Act
            var result = await controller.CalculateTimeAngle(null, 3, 60);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.NotNull(badRequestResult.Value);
        }

        [Fact]
        public async Task CalculateTimeAngle_PassesCorrectRequestToValidatorAndService()
        {
            // Arrange
            TimeAngleRequest? capturedValidatorRequest = null;
            TimeAngleRequest? capturedServiceRequest = null;

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TimeAngleRequest>(), default))
                .Callback<TimeAngleRequest, CancellationToken>((req, _) => capturedValidatorRequest = req)
                .ReturnsAsync(new ValidationResult());

            _serviceMock
                .Setup(s => s.Calculate(It.IsAny<TimeAngleRequest>()))
                .Callback<TimeAngleRequest>(req => capturedServiceRequest = req)
                .Returns(new TimeAngleResponse
                {
                    Hour = 3,
                    Minutes = 15,
                    Angle = 7.5
                });

            var controller = new TimeAngleApiController(
                _serviceMock.Object,
                _validatorMock.Object);

            // Act
            await controller.CalculateTimeAngle(null, 3, 15);

            // Assert
            Assert.NotNull(capturedValidatorRequest);
            Assert.Equal(3, capturedValidatorRequest!.Hour);
            Assert.Equal(15, capturedValidatorRequest.Minutes);

            Assert.NotNull(capturedServiceRequest);
            Assert.Equal(3, capturedServiceRequest!.Hour);
            Assert.Equal(15, capturedServiceRequest.Minutes);
        }

        [Fact]
        public async Task CalculateTimeAngle_WhenValidationFails_DoesNotCallService()
        {
            // Arrange
            var failures = new List<ValidationFailure>
            {
                new("Hour", "Hour is required.")
            };

            _validatorMock
                .Setup(v => v.ValidateAsync(It.IsAny<TimeAngleRequest>(), default))
                .ReturnsAsync(new ValidationResult(failures));

            var controller = new TimeAngleApiController(
                _serviceMock.Object,
                _validatorMock.Object);

            // Act
            await controller.CalculateTimeAngle(null, null, null);

            // Assert
            _serviceMock.Verify(s => s.Calculate(It.IsAny<TimeAngleRequest>()), Times.Never);
        }
    }
}