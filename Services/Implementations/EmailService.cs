using MailKit.Security;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Win32;
using MimeKit;
using MimeKit.Text;
using Services.Interfaces;
using Services.Models;
using Services.Models.User;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Threading;
using Twilio.TwiML.Messaging;

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
        var formattedStartTime = request.StartTime.Day.ToString() + GetDaySuffix(request.StartTime.Day) +
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

    public void SendLoginCode(LoginCodeDto request)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
        email.To.Add(MailboxAddress.Parse(request.Email));
        email.Subject = "Login Attempt";
        var emailBody = $@"
        <table width='100%' cellpadding='0' cellspacing='0' style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <tr>
                <td style='padding: 20px;'>
                    <p style='font-size: 16px; margin-bottom: 20px;'>Hello, {request.FirstName}!</p>
                    <p style='margin-bottom: 20px;'>We received a login request on your behalf.</p>
                    <table cellpadding='0' cellspacing='0' style='margin: 20px 0;'>
                        <tr>
                            <td style='padding: 15px; background-color: #f9f9f9; border: 1px solid #ddd; text-align: center;'>
                                <h1 style='font-size: 24px; color: #0056b3; margin: 0;'>Your Code: <strong>{request.Code}</strong></h1>
                            </td>
                        </tr>
                    </table>
                    <p>If you are not trying to login, please contact us immediately.</p>
                    <p style='margin-top: 20px;'>Thank you,<br>The Support Team</p>
                </td>
            </tr>
        </table>";

        email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_config.GetSection("EmailUserName").Value, _config.GetSection("EmailPassword").Value);
        smtp.Send(email);
        smtp.Disconnect(true);
    }

    public void SendPasswordChangeCode(PasswordChangeAttemptDto request)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
        email.To.Add(MailboxAddress.Parse(request.Email));
        email.Subject = "Password change attempt";
        var emailBody = $@"
        <table width='100%' cellpadding='0' cellspacing='0' style='font-family: Arial, sans-serif; line-height: 1.6; color: #333;'>
            <tr>
                <td style='padding: 20px;'>
                    <p style='font-size: 16px; margin-bottom: 20px;'>Hello, {request.FirstName}!</p>
                    <p style='margin-bottom: 20px;'>We received a request to change your password.</p>
                    <table cellpadding='0' cellspacing='0' style='margin: 20px 0;'>
                        <tr>
                            <td style='padding: 15px; background-color: #f9f9f9; border: 1px solid #ddd; text-align: center;'>
                                <h1 style='font-size: 24px; color: #0056b3; margin: 0;'>Your Code: <strong>{request.Code}</strong></h1>
                            </td>
                        </tr>
                    </table>
                    <p>If you did not request a password change, please contact us immediately.</p>
                    <p style='margin-top: 20px;'>Thank you,<br>The Support Team</p>
                </td>
            </tr>
        </table>";

        email.Body = new TextPart(TextFormat.Html) { Text = emailBody };

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
        var formattedStartTime = request.StartTime.Day.ToString() + GetDaySuffix(request.StartTime.Day) +
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

    public void SendInvitationEmail(string emailTo, string clinicName)
    {
        var email = new MimeMessage();
        email.From.Add(MailboxAddress.Parse(_config.GetSection("EmailUserName").Value));
        email.To.Add(MailboxAddress.Parse(emailTo));
        email.Subject = "Invitation to ORAXS";

        var registrationLink = "";

        string emailTemplate = $@"
            <!DOCTYPE html>
            <html lang='en'>
            <head>
                <meta charset='UTF-8'>
                <title>ORAXS Invitation</title>
            </head>
            <body style='font-family: Arial, sans-serif; background-color: #f9fafb; margin: 0; padding: 0;'>
                <table align='center' width='100%' cellpadding='0' cellspacing='0' style='max-width:600px; background-color:#ffffff; padding:20px; border-radius:8px; border:1px solid #eaeaea;'>
                    <tr>
                        <td align='center' style='padding-bottom: 20px; border-bottom: 1px solid #eaeaea;'>
                            <h2 style='margin:0;'>Join ORAXS!</h2>
                        </td>
                    </tr>
                    <tr>
                        <td style='padding-top:20px;'>
                            <p>Hello,</p>
                            <p>You have been invited by the administrator of <strong>{clinicName}</strong> to join ORAXS, a dental app to simplify your practice.</p>
                            <p>To complete your registration, click the link below:</p>
                            <p style='text-align: center; margin: 30px 0;'>
                                <a href='{registrationLink}' style='background-color:#2d89ef; color:#ffffff; padding:10px 20px; text-decoration:none; border-radius:4px;'>Register Now</a>
                            </p>
                            <p>If you didn't expect this email, please ignore it.</p>
                        </td>
                    </tr>
                    <tr>
                        <td align='center' style='font-size:12px; color:#888888; padding-top:20px;'>
                            � ORAXS Dental App | All Rights Reserved
                        </td>
                    </tr>
                </table>
            </body>
            </html>
            ";

        email.Body = new TextPart(TextFormat.Html) { Text = emailTemplate };
        using var smtp = new MailKit.Net.Smtp.SmtpClient();
        smtp.Connect(_config.GetSection("EmailHost").Value, 587, SecureSocketOptions.StartTls);
        smtp.Authenticate(_config.GetSection("EmailUserName").Value, _config.GetSection("EmailPassword").Value);

        try
        {
            smtp.Send(email);
        }
        catch (Exception)
        {
            throw new Exception("Invitation email could not be send!");
        }

        smtp.Disconnect(true);
    }

    private string GetDaySuffix(int day){
        return (day % 10 == 1 && day != 11) ? "st" :
            (day % 10 == 2 && day != 12) ? "nd" :
            (day % 10 == 3 && day != 13) ? "rd" : "th";
    }
}