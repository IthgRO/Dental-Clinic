using Services.Models;
using Services.Models.User;

namespace Services.Interfaces;

public interface IEmailService
{
    void SendEmail(EmailDto request);

    void SendReminderEmail(EmailDto request);

    void SendPasswordChangeCode(PasswordChangeAttemptDto request);

    void SendLoginCode(LoginCodeDto request);

    void SendInvitationEmail(string emailTo, string clinicName);
}

