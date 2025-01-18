using Services.Models;
using Services.Models.User;

namespace Services.Interfaces;

public interface IEmailService
{
    void SendEmail(EmailDto request);

    void SendReminderEmail(EmailDto request);

    void SendPasswordChangeCode(PasswordChangeAttemptDto request);
}
