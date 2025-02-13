using Services.Models;

namespace Services.Interfaces
{
    public interface ISmsService
    {
        void SendSms(SmsDto request);
        void SendSmsReminder(SmsDto request);
    }
}
