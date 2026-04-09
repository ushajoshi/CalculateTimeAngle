using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TimeAngle.Models;
using TimeAngle.Services;

namespace TimeAngle.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class TimeAngleApiController : ControllerBase
    {
        private readonly ITimeAngleService _timeAngleService;
        private readonly IValidator<TimeAngleRequest> _validator;

        public TimeAngleApiController(
            ITimeAngleService timeAngleService,
            IValidator<TimeAngleRequest> validator)
        {
            _timeAngleService = timeAngleService;
            _validator = validator;
        }

        [HttpGet("CalculateTimeAngle")]
        public async Task<IActionResult> CalculateTimeAngle(
            [FromQuery] string? time,
            [FromQuery] int? hour,
            [FromQuery] int? minutes)
        {
            var request = new TimeAngleRequest
            {
                Time = time,
                Hour = hour,
                Minutes = minutes
            };

            var validationResult = await _validator.ValidateAsync(request);

            if (!validationResult.IsValid)
            {
                return BadRequest(new
                {
                    errors = validationResult.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }

            var result = _timeAngleService.Calculate(request);

            return Ok(result);
        }
    }
}