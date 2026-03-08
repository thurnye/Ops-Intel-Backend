using OperationIntelligence.DB.Entities;
using System.Threading.Tasks;

namespace OperationIntelligence.DB.Repositories
{
    public interface IAuthRepository
    {
        Task<PlatformUser?> FindByEmailAsync(string email);
        Task<PlatformUser> CreateUserAsync(PlatformUser user, string password);
        Task<bool> CheckPasswordAsync(PlatformUser user, string password);
    }

}
