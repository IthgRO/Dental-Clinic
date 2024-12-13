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

    public EmailService(IConfiguration config){
        _config = config;
    }
    public void SendEmail(EmailDto request){
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
        email.To.Add(MailboxAddress.Parse(request.To));
        var emailBody = $@"
                <h1>Confirmation Email</h1>
                <p>This is a confirmation email. You will find all the information required for your appointment below.</p>
                <hr>
                <p><strong>Appointment Details:</strong></p>
                <ul>
                    <li>Start Time: {request.StartTime}</li>
                    <li>Clinic ID: {request.ClinicId}</li>
                    <li>Service ID: {request.ServiceId}</li>
                    <li>Dentist ID: {request.DentistId}</li>
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

