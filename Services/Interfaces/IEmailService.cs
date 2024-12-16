using System;
using Services.Models;

namespace Services.Interfaces;

public interface IEmailService
{
    void SendEmail(EmailDto request);
    void SendReminderEmail(EmailDto request);
}
