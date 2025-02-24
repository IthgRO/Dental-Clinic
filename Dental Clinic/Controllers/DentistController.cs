using AutoMapper;
using Dental_Clinic.Dtos;
using Dental_Clinic.Enums;
using Dental_Clinic.Requests.Appointment;
using Dental_Clinic.Requests.Dentist;
using Dental_Clinic.Responses.Appointment;
using Dental_Clinic.Responses.Dentist;
using Dental_Clinic.Responses.User;
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

                // Create the appointment and store it in the database
                var bookedAppointment = await _appointmentService.BookAppointment(userId, request.DentistId, request.ServiceId, request.ClinicId, startDateUtc);

                return Ok(bookedAppointment);
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
        [HttpGet("seeDentistsAppointments")]
        public async Task<IActionResult> SeeDentistAppointments()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                var appointments = await _appointmentService.GetDentistAppointments(userId);

                return Ok(_mapper.Map<List<DentistAppointmentViewModel>>(appointments));
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

        [Authorize]
        [HttpPost("confirmAppointment")]
        public async Task<IActionResult> ConfirmAppointment(ConfirmAppointmentRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                // Confirm the appointment (this method should update the appointment's status to Confirmed)
                // and return the confirmed appointment details.
                await _appointmentService.ConfirmAppointment(userId, request.AppointmentId);

                // Retrieve the confirmed appointment details from the DB
                var confirmedAppointment = await _appointmentService.GetAppointmentByIdAsync(request.AppointmentId);
                if (confirmedAppointment == null)
                    return NotFound("Appointment not found.");

                // Retrieve additional details required for notifications
                var clinic = await _clinicService.GetClinicByIdAsync(confirmedAppointment.ClinicId);
                if (clinic == null)
                    return NotFound("Clinic not found.");

                var currentUser = await _userService.GetUserByIdAsync(confirmedAppointment.PatientId);
                if (currentUser == null)
                    return NotFound("User not found.");
                    
                // Get the user's preferred reminder type (could be Email, SMS, or Both)
                var reminderType = _userService.GetUserReminderType(confirmedAppointment.PatientId);

                // Create a reminder to be sent 1 hour before the appointment.
                var reminder = new ReminderDto
                {
                    AppointmentId = confirmedAppointment.Id,
                    Type = reminderType,
                    Status = ReminderStatus.Pending,
                    Timezone = _clinicService.ConvertClinicTimezoneEnumToWindows(clinic.Timezone),
                    SendAt = confirmedAppointment.StartTime.AddHours(-1), // stored as UTC
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Appointment = confirmedAppointment
                };

                await _reminderService.CreateReminderAsync(reminder);

                // Prepare and send the Email Notification
                var emailDto = new EmailDto
                {
                    DentistId = confirmedAppointment.DentistId,
                    ClinicId = confirmedAppointment.ClinicId,
                    ServiceId = confirmedAppointment.ServiceId,
                    StartTime = confirmedAppointment.StartTime, // display local time
                    To = currentUser.Email,
                    Status = AppointmentStatus.Confirmed
                };

                _emailService.SendEmail(emailDto);

                // Prepare and send the SMS Notification
                var smsDto = new SmsDto
                {
                    To = currentUser.Phone,
                    StartTime = confirmedAppointment.StartTime, // display local time
                    ClinicId = confirmedAppointment.ClinicId,
                    ServiceId = confirmedAppointment.ServiceId,
                    DentistId = confirmedAppointment.DentistId,
                    Status = AppointmentStatus.Confirmed
                };

                _smsService.SendSms(smsDto);

                return Ok(confirmedAppointment);
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }


        [HttpPost("sendDoctorLoginCode")]
        public async Task<IActionResult> SendDoctorLoginCode(SendLoginCodeRequest request)
        {
            try
            {
                var code = await _userService.GetLoginCode(request.Email, request.Password);
                _emailService.SendLoginCode(code);

                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [HttpPost("loginWithCode")]
        public async Task<IActionResult> LoginWithCode(LoginWithCodeRequest request)
        {
            try
            {
                var result = await _userService.LoginWithCode(request.Email, request.Password, request.Code);
                return Ok(_mapper.Map<LoginResponse>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }
    }
}
