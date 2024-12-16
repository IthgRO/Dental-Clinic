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
        var formattedStartTime = request.StartTime.ToString("dd") + GetDaySuffix(request.StartTime.Day) +
                         " of " + request.StartTime.ToString("MMMM h:mm tt");
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = "Appointment Confirmation";
        var emailBody = $@"
                <h1>Your Appointment is Confirmed!</h1>
                <p>Your appointment has been successfully scheduled. Below are the details of your appointment:</p>
                <hr>
                <p><strong>Appointment Details:</strong></p>
                <ul>
                    <li>Date: {formattedStartTime}</li>
                    <li>Clinic: {clinicName}</li>
                    <li>Service: {serviceName}</li>
                    <li>Dentist: {dentistName}</li>
                </ul>
            ";
        email.Body = new TextPart(TextFormat.Html){Text = emailBody};
            
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_config.GetSection("EmailUserName").Value, _config.GetSection("EmailPassword").Value);
        smtp.Send(email);
        smtp.Disconnect(true);
    }
    public void SendReminderEmail(EmailDto request){
        var clinicName = _clinicService.GetClinicNameById(request.ClinicId);
        var serviceName = _serviceService.GetServiceNameById(request.ServiceId);
        var dentistName = _dentistService.GetDentistNameById(request.DentistId);
        var formattedStartTime = request.StartTime.ToString("dd") + GetDaySuffix(request.StartTime.Day) +
                         " of " + request.StartTime.ToString("MMMM h:mm tt");
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
        email.To.Add(MailboxAddress.Parse(request.To));
        email.Subject = "Appointment Reminder";
        var emailBody = $@"
                <h1>Dentist Appointment Reminder</h1>
                <p>This is a reminder that you have a dentist appointment in 1 hour! Below are the details of your appointment:</p>
                <hr>
                <p><strong>Appointment Details:</strong></p>
                <ul>
                    <li>Date: {formattedStartTime}</li>
                    <li>Clinic: {clinicName}</li>
                    <li>Service: {serviceName}</li>
                    <li>Dentist: {dentistName}</li>
                </ul>
                <hr>
                <p>Please arrive a few minutes early to allow time for check-in. We look forward to seeing you soon!</p>
            ";
        email.Body = new TextPart(TextFormat.Html){Text = emailBody};
            
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_config.GetSection("EmailUserName").Value, _config.GetSection("EmailPassword").Value);
        smtp.Send(email);
        smtp.Disconnect(true);
    }

    private string GetDaySuffix(int day){
        return (day % 10 == 1 && day != 11) ? "st" :
            (day % 10 == 2 && day != 12) ? "nd" :
            (day % 10 == 3 && day != 13) ? "rd" : "th";
    }
}