using AutoMapper;
using Dental_Clinic.Requests.Service;
using Dental_Clinic.Responses.Service;
using Dental_Clinic.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.Service;
using System.Security.Claims;

namespace Dental_Clinic.Controllers
{
    [Route("api/service")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private readonly IServiceService _serviceService;
        private readonly IMapper _mapper;
        public ServiceController(IServiceService serviceService, IMapper mapper) 
        {
            _serviceService = serviceService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("getAvailableServices")]
        public IActionResult GetAvailableServices()
        {
            try
            {
                return Ok(ServiceUtils.GetAvailableServices());
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("addServiceToDentist")]
        public async Task<IActionResult> AddServiceToDentist(AddServiceToDentistRequest request)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                ServiceUtils.VerifyIfServiceIsApproved(_mapper.Map<AvailableServiceViewModel>(request));
                var serviceDto = _mapper.Map<ServiceDto>(request);
                serviceDto.DentistId = userId;

                await _serviceService.AddServiceToDentist(serviceDto);

                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpGet("seeDentistServices")]
        public async Task<IActionResult> GetDentistServices()
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                var services = await _serviceService.GetServicesForDentist(userId);

                return Ok(_mapper.Map<List<DentistServiceViewModel>>(services));
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }

        [Authorize]
        [HttpPost("updateDentistServices")]
        public async Task<IActionResult> UpdateDentistServices(List<AddServiceToDentistRequest> services)
        {
            try
            {
                var user = User.FindFirstValue(ClaimTypes.Actor);
                var userId = Int32.Parse(user);

                ServiceUtils.VerifyIfListOfServicesIsApproved(_mapper.Map<List<AvailableServiceViewModel>>(services));

                await _serviceService.UpdateServicesForDentist(_mapper.Map<List<ServiceDto>>(services), userId);

                return Ok();
            }
            catch (Exception ex)
            {
                return ErrorResponse.GetErrorResponse(ex);
            }
        }
    }
}
