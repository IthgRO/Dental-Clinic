using AutoMapper;
using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;
using Dental_Clinic.Requests.Appointment;
using Dental_Clinic.Responses.Appointment;
using Dental_Clinic.Responses.Dentist;
using Dental_Clinic.Utils;
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
        private readonly IClinicService _clinicService;
        private readonly ISmsService _smsService;

        public DentistController(IUserService userService, ISmsService smsService, IAppointmentService appointmentService, IEmailService emailService, IReminderService reminderService, IMapper mapper, IClinicService clinicService)
        {
            _userService = userService;
            _emailService = emailService;
            _appointmentService = appointmentService;
            _mapper = mapper;
            _reminderService = reminderService;
            _clinicService = clinicService;
            _smsService = smsService;
        }

        [HttpGet("seeAvailableDentists")]
        public async Task<IActionResult> GetAvailableDentists()
        {
            try
            {
                var dentists = await _userService.GetAvailableDentists();

                return Ok(_mapper.Map<AvailableDentistsResponse>(dentists));
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [HttpPost("seeAvailableSlots")]
        public async Task<IActionResult> GetAvailableSlots(GetFreeSlotsRequest request)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableTimeSlots(request.DentistId, request.StartDate, request.EndDate);

                return Ok(slots);
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("bookAppointment")]
        public async Task<IActionResult> BookAppointment(BookAppointmentRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);
                var currentUser = await _userService.GetUserByIdAsync(userId);
                if (currentUser == null)
                {
                    return NotFound("User not found.");
                }

                var clinic = await _clinicService.GetClinicByIdAsync(request.ClinicId);
                if (clinic == null)
                {
                    return NotFound("Clinic not found.");
                }

                // Convert StartDate (Clinic's Local Time) to UTC
                var startDateUtc = _clinicService.ConvertToUtc(request.ClinicId, request.StartDate);

                var booked_appointment = await _appointmentService.BookAppointment(userId, request.DentistId, request.ServiceId, request.ClinicId, startDateUtc);
                var reminderType = _userService.GetUserReminderType(userId);
                var reminder = new ReminderDto
                {
                    AppointmentId = booked_appointment.Id,
                    Type = reminderType,
                    Status = ReminderStatus.Pending,
                    Timezone = _clinicService.ConvertClinicTimezoneEnumToWindows(clinic.Timezone),
                    SendAt = startDateUtc.AddHours(-1),
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
                var smsDto = new SmsDto
                {
                    To = currentUser.Phone,
                    StartTime = request.StartDate,
                    ClinicId = request.ClinicId,
                    ServiceId = request.ServiceId,
                    DentistId = request.DentistId,
                    Status = AppointmentStatus.Pending
                };

                _smsService.SendSms(smsDto);
                    return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("updateAppointment")]
        public async Task<IActionResult> UpdateAppointment(UpdateAppointmentRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                await _appointmentService.UpdateAppointment(userId, request.AppointmentId, request.NewDate);
                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpGet("seeMyAppointments")]
        public async Task<IActionResult> SeeMyAppointments()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                var appointments = await _appointmentService.GetMyAppointments(userId);

                return Ok(_mapper.Map<List<AppointmentViewModel>>(appointments));
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("cancelAppointment")]
        public async Task<IActionResult> CancelAppointment(CancelAppointmentRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                await _appointmentService.CancelAppointment(userId, request.AppointmentId);
                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

    }
}
