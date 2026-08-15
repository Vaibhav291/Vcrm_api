using Vcrm.Models;

namespace Vcrm.Repositories;

public interface IUserRepository
{
    Task<User?> GetByUsernameAsync(string username);

    Task<User?> GetByEmailAsync(string email);

    Task<User> InsertAsync(User user);

    Task<User> SetOtpAsync(int userId, string otpCode, DateTime otpExpiresAt);

    Task<User?> VerifyEmailAsync(int userId, string otpCode);

    Task DeleteUnverifiedUserAsync(int userId);
}
