using AutoMapper;
using Dental_Clinic.Requests.User;
using Dental_Clinic.Responses.User;
using Microsoft.AspNetCore.Mvc;
using Services.Interfaces;
using Services.Models.User;

namespace Dental_Clinic.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IMapper _mapper;


        public UserController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost("register")]
        public async Task<IActionResult> RegisterUser(RegisterUserRequest request)
        {
            await _userService.RegisterUser(_mapper.Map<RegisterUserDto>(request));

            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserRequest request)
        {
            var jwt = await _userService.Login(_mapper.Map<LoginUserDto>(request));

            return Ok(new LoginResponse { Jwt = jwt});
        }
    }
}
