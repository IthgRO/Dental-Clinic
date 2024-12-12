using AutoMapper;
using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;
using Dental_Clinic.Requests.Appointment;
using Dental_Clinic.Responses.Appointment;
using Dental_Clinic.Responses.Dentist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models;
using System.Security.Claims;

namespace Dental_Clinic.Controllers
{

    [Route("api/dentist")]
    [ApiController]
    public class DentistController : Controller
    {
        private readonly IUserService _userService;
        private readonly IAppointmentService _appointmentService;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IReminderService _reminderService;

        public DentistController(IUserService userService, IAppointmentService appointmentService, IEmailService emailService, IMapper mapper)
        {
            _userService = userService;
            _emailService = emailService;
            _appointmentService = appointmentService;
            _mapper = mapper;
        }

        [HttpGet("seeAvailableDentists")]
        public async Task<IActionResult> GetAvailableDentists()
        {
            var dentists = await _userService.GetAvailableDentists();
            
            return Ok(_mapper.Map<AvailableDentistsResponse>(dentists));
        }

        [HttpPost("seeAvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots(GetFreeSlotsRequest request)
        {
            var slots = await _appointmentService.GetAvailableTimeSlots(request.DentistId, request.StartDate, request.EndDate);

            return Ok(slots);
        }

        [Authorize]
        [HttpPost("bookAppointment")]
        public async Task<IActionResult> BookAppointment(BookAppointmentRequest request)
        {
            var user = User.FindFirstValue(ClaimTypes.Actor);
            var userId = Int32.Parse(user);
            var currentUser = await _userService.GetUserByIdAsync(userId);
            if (currentUser == null)
            {
                return NotFound("User not found.");
            }
            
            var booked_appointment = await _appointmentService.BookAppointment(userId, request.DentistId, request.ServiceId, request.ClinicId, request.StartDate);
            
            var reminder = new ReminderDto
            {
                AppointmentId = booked_appointment.Id,
                Type = ReminderType.Email, 
                Status = ReminderStatus.Pending,
                Timezone = "Pacific Standard Time",
                SendAt = request.StartDate.AddHours(-1), // 1 hour before the appointment
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Appointment = booked_appointment
            };

            await _reminderService.CreateReminderAsync(reminder);
            var emailDto = new EmailDto
            {
                DentistId = request.DentistId,
                ClinicId = request.ClinicId,
                ServiceId = request.ServiceId,
                StartTime = request.StartDate,
                To = currentUser.Email,
                Status = Enums.AppointmentStatus.Pending

            };
            _emailService.SendEmail(emailDto);
            return Ok();
        }

        [Authorize]
        [HttpGet("seeMyAppointments")]
        public async Task<IActionResult> SeeMyAppointments()
        {
            var user = User.FindFirstValue(ClaimTypes.Actor);
            var userId = Int32.Parse(user);

            var appointments = await _appointmentService.GetMyAppointments(userId);

            return Ok(_mapper.Map<List<AppointmentViewModel>>(appointments));
        }

        [Authorize]
        [HttpPost("cancelAppointment")]
        public async Task<IActionResult> CancelAppointment(CancelAppointmentRequest request)
        {
            var user = User.FindFirstValue(ClaimTypes.Actor);
            var userId = Int32.Parse(user);

            await _appointmentService.CancelAppointment(userId, request.AppointmentId);
            return Ok();
        }

    }
}
