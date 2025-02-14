using AutoMapper;
using Dental_Clinic.Requests.Clinic;
using Dental_Clinic.Responses.Clinic;
using Dental_Clinic.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using System.Security.Claims;

namespace Dental_Clinic.Controllers
{
    [Route("api/clinic")]
    [ApiController]
    public class ClinicController : Controller
    {
        private readonly IClinicService _clinicService;
        private readonly IMapper _mapper;

        public ClinicController(IClinicService clinicService, IMapper mapper)
        {
            _clinicService = clinicService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("getMyClinic")]
        public async Task<IActionResult> GetClinicForDentist()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                var clinic = await _clinicService.GetDentistClinic(userId);

                return Ok(_mapper.Map<DentistClinicViewModel>(clinic));

            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("updateClinicAddress")]
        public async Task<IActionResult> UpdateClinicddress(UpdateClinicAddressRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                await _clinicService.UpdateClinicAddress(userId, request.Address);

                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }
    }
}
