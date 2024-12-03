using System.Net.Mail;
using MailKit.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using Services.Interfaces;
using Services.Implementations;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Services.Models;

namespace Dental_Clinic.Controllers
{
    [Route("api/sendemail")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;
        public EmailController(IEmailService emailService){
            _emailService = emailService;
        }
        [HttpPost]
        public IActionResult SendEmail(EmailDto request){
            _emailService.SendEmail(request);

            return Ok();
        }
    }
}
