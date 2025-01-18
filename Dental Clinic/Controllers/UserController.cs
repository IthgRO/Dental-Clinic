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
        private readonly IEmailService _emailService;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;


        public UserController(IUserService userService, IPasswordService passwordService, IEmailService emailService, IMapper mapper)
        {
            _userService = userService;
            _passwordService = passwordService;
            _emailService = emailService;
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
            var result = await _userService.Login(_mapper.Map<LoginUserDto>(request));

            return Ok(_mapper.Map<LoginResponse>(result));
        }

        [HttpPost("sendPasswordChangeCode")]
        public async Task<IActionResult> SendPasswordChangeCode(SendPasswordChangeCodeRequest request)
        {
            var code = await _passwordService.GetPasswordChangeCode(request.Email);
            _emailService.SendPasswordChangeCode(code);
            return Ok();
        }

        [HttpPost("changeForgottenPassword")]
        public async Task<IActionResult> ChangeForgottenPassword(ChangeForgottenPasswordRequest request)
        {
            await _passwordService.ChangeForgottenPassword(request.Email, request.Code, request.NewPassword);
            return Ok();
        }
    }
}
