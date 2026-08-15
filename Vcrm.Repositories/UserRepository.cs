using Microsoft.EntityFrameworkCore;
using Vcrm.Data;
using Vcrm.Models;

namespace Vcrm.Repositories;

public class UserRepository : IUserRepository
{
    private readonly VcrmDbContext _context;

    public UserRepository(VcrmDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        var results = await _context.Users
            .FromSqlInterpolated($"EXEC dbo.GetUserByUsername {username}")
            .AsNoTracking()
            .ToListAsync();

        return results.FirstOrDefault();
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        var results = await _context.Users
            .FromSqlInterpolated($"EXEC dbo.GetUserByEmail {email}")
            .AsNoTracking()
            .ToListAsync();

        return results.FirstOrDefault();
    }

    public async Task<User> InsertAsync(User user)
    {
        var inserted = await _context.Users
            .FromSqlInterpolated($@"EXEC dbo.InsertUser
                {user.Username}, {user.Email}, {user.PasswordHash}, {user.Role}, {user.CreatedAt},
                {user.IsEmailVerified}, {user.OtpCode}, {user.OtpExpiresAt}")
            .AsNoTracking()
            .ToListAsync();

        return inserted.Single();
    }

    public async Task<User> SetOtpAsync(int userId, string otpCode, DateTime otpExpiresAt)
    {
        var updated = await _context.Users
            .FromSqlInterpolated($"EXEC dbo.SetUserOtp {userId}, {otpCode}, {otpExpiresAt}")
            .AsNoTracking()
            .ToListAsync();

        return updated.Single();
    }

    public async Task<User?> VerifyEmailAsync(int userId, string otpCode)
    {
        var results = await _context.Users
            .FromSqlInterpolated($"EXEC dbo.VerifyUserEmail {userId}, {otpCode}")
            .AsNoTracking()
            .ToListAsync();

        return results.FirstOrDefault();
    }

    public Task DeleteUnverifiedUserAsync(int userId)
    {
        return _context.Database.ExecuteSqlInterpolatedAsync($"EXEC dbo.DeleteUnverifiedUser {userId}");
    }
}
