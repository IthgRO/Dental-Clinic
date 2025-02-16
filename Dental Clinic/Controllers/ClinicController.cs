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

        [Authorize]
        [HttpPost("uploadClinicPicture")]
        public async Task<IActionResult> UpdateClinicPicture(IFormFile picture)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                ValidatePicture(picture);

                using (var memoryStream = new MemoryStream())
                {
                    await picture.CopyToAsync(memoryStream);

                    byte[] imageBytes = memoryStream.ToArray();

                    await _clinicService.UploadClinicPicture(userId, imageBytes, picture.ContentType);

                    
                }

                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize, HttpGet("getClinicPicture")]
        public async Task<IActionResult> GetClinicPicture()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                var picture = await _clinicService.GetClinicPicture(userId);

                return File(picture.Picture, picture.Format);
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        private void ValidatePicture(IFormFile picture)
        {
            if (picture == null || picture.Length == 0)
            {
                throw new Exception("Picture for clinic not valid!");
            }
        }
    }
}
