using AutoMapper;
using Dental_Clinic.Responses.Dentist;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;

namespace Dental_Clinic.Controllers
{

    [Route("api/dentist")]
    [ApiController]
    public class DentistController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public DentistController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [Authorize]
        [HttpGet("seeAvailableDentists")]
        public async Task<IActionResult> GetAvailableDentists()
        {
            var dentists = await _userService.GetAvailableDentists();
            
            return Ok(_mapper.Map<AvailableDentistsResponse>(dentists));
        }
    }
}
