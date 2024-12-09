using Microsoft.AspNetCore.Identity;

namespace MoviesAPI_Minimal.Services.Interface
{
    public interface IUsersService
    {
        Task<IdentityUser?> GetUser();
    }
}