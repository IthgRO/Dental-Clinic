using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MimeKit.Text;
using Services.Interfaces;
using Services.Models;

namespace Services.Implementations;

public class EmailService: IEmailService
{
    private readonly IConfiguration _config;
    private readonly IClinicService _clinicService; 
    private readonly IServiceService _serviceService; 
    private readonly IDentistService _dentistService;

    public EmailService(IConfiguration config, IClinicService clinicService,IServiceService serviceService,IDentistService dentistService){
        _config = config;
        _clinicService = clinicService;
        _serviceService = serviceService;
        _dentistService = dentistService;
    }
    public void SendEmail(EmailDto request){
        var clinicName = _clinicService.GetClinicNameById(request.ClinicId);
        var serviceName = _serviceService.GetServiceNameById(request.ServiceId);
        var dentistName = _dentistService.GetDentistNameById(request.DentistId);
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = "Appointment Confirmation";
        var emailBody = $@"
                <h1>>Your Appointment is Confirmed!</h1>
                <p>Your appointment has been successfully scheduled. Below are the details of your appointment:</p>
                <hr>
                <p><strong>Appointment Details:</strong></p>
                <ul>
                    <li>Start Time: {request.StartTime}</li>
                    <li>Clinic ID: {clinicName}</li>
                    <li>Service ID: {serviceName}</li>
                    <li>Dentist ID: {dentistName}</li>
                </ul>
            ";
        email.Body = new TextPart(TextFormat.Html){Text = emailBody};
            
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_config.GetSection("EmailUserName").Value, _config.GetSection("EmailPassword").Value);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
}