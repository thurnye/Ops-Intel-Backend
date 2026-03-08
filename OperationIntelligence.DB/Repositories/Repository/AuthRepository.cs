using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OperationIntelligence.DB.Entities;

namespace OperationIntelligence.DB.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly OperationIntelligenceDbContext _context;
        private readonly PasswordHasher<PlatformUser> _passwordHasher = new();

        public AuthRepository(OperationIntelligenceDbContext context)
        {
            _context = context;
        }

        public async Task<PlatformUser?> FindByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<PlatformUser> CreateUserAsync(PlatformUser user, string password)
        {
            var existing = await _context.Users.AnyAsync(u => u.Email == user.Email);
            if (existing)
            {
                throw new Exception("A user with this email already exists.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, password);
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return user;
        }

        public Task<bool> CheckPasswordAsync(PlatformUser user, string password)
        {
            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
            return Task.FromResult(result == PasswordVerificationResult.Success || result == PasswordVerificationResult.SuccessRehashNeeded);
        }
    }
}
