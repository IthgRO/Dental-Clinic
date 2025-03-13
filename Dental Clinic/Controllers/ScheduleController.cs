using AutoMapper;
using Dental_Clinic.Requests.Schedules;
using Dental_Clinic.Responses.Schedules;
using Dental_Clinic.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace Dental_Clinic.Controllers
{
    [Route("api/schedule")]
    [ApiController]
    public class ScheduleController : Controller
    {
        private IScheduleService _scheduleService;
        private readonly IMapper _mapper;

        public ScheduleController(IScheduleService scheduleService, IMapper mapper) 
        {
            _scheduleService = scheduleService;
            _mapper = mapper;
        }

        [HttpPost("addSchedule")]
        [Authorize]
        public async Task<IActionResult> AddSchedule(AddScheduleRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                await _scheduleService.AddSchedule(userId, request.StartTime, request.EndTime, request.DayOfWeek);
                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [HttpPost("deleteSchedule")]
        [Authorize]
        public async Task<IActionResult> DeleteSchedule(DeleteScheduleRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                await _scheduleService.DeleteSchedule(userId, request.ScheduleId);
                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [HttpGet("getMySchedules")]
        [Authorize]
        public async Task<IActionResult> GetMySchedules()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                var result = await _scheduleService.GetDentistSchedules(userId);
                return Ok(_mapper.Map<List<ScheduleViewModel>>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }
    }
}
