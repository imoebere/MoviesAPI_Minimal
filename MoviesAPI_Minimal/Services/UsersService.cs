using Microsoft.AspNetCore.Identity;
using MoviesAPI_Minimal.Services.Interface;

namespace MoviesAPI_Minimal.Services
{
    public class UsersService : IUsersService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<IdentityUser> _userManager;

        public UsersService(IHttpContextAccessor httpContextAccessor, UserManager<IdentityUser> userManager)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        public async Task<IdentityUser?> GetUser()
        {
            var emailClaims = _httpContextAccessor.HttpContext!.User.Claims.Where(x => x.Type == "email")
                              .FirstOrDefault();
            if (emailClaims is null)
            {
                return null;
            }

            var email = emailClaims.Value;
            return await _userManager.FindByEmailAsync(email);
        }
    }
}
