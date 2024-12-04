using Microsoft.AspNetCore.Identity;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IUserRepository
    {
        Task<string> Create(IdentityUser user);
        Task<IdentityUser?> GetByEmail(string normalizedEmail);
    }
}