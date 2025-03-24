using AutoMapper;
using Dental_Clinic.Requests.Invitation;
using Dental_Clinic.Responses.Invitation;
using Dental_Clinic.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using System.Security.Claims;

namespace Dental_Clinic.Controllers
{
    [Route("api/invitation")]
    [ApiController]
    public class InvitationController : Controller
    {
        private readonly IInvitationService _invitationService;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;

        public InvitationController(IInvitationService invitationService, IEmailService emailService, IMapper mapper)
        {
            _invitationService = invitationService;
            _mapper = mapper;
            _emailService = emailService;
        }

        [Authorize]
        [HttpPost("sendInvitation")]
        public async Task<IActionResult> InviteDentist(InviteDentistRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var adminId = Int32.Parse(user);

                var result = await _invitationService.SendInvitationToDentist(request.Email, adminId);

                _emailService.SendInvitationEmail(request.Email, result.ClinicName);
                
                return Ok(_mapper.Map<InvitationViewModel>(result));
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("cancelInvitation")]
        public async Task<IActionResult> CancelInvitation(CancelInvitationRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var adminId = Int32.Parse(user);

                await _invitationService.CancelInvitation(adminId, request.InvitationId);
                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpGet("seeInvitations")]
        public async Task<IActionResult> SeeMyInvitations()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var adminId = Int32.Parse(user);

                var invitations = await _invitationService.GetAdminInvitations(adminId);

                return Ok(_mapper.Map<List<InvitationViewModel>>(invitations));
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }
    }
}
