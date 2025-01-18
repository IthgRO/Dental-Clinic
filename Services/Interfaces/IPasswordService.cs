using Services.Models.User;

namespace Services.Interfaces
{
    public interface IPasswordService
    {
        Task<PasswordChangeAttemptDto> GetPasswordChangeCode(string email);
        Task ChangeForgottenPassword(string email, string code, string newPassword);
    }
}
