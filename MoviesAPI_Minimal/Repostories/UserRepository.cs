using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MoviesAPI_Minimal.Repostories.Interface;
using System.Data;
using System.Security.Claims;

namespace MoviesAPI_Minimal.Repostories
{
    public class UserRepository : IUserRepository
    {
        private string connetionString;

        public UserRepository(IConfiguration configuration)
        {
            connetionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<IdentityUser?> GetByEmail(string normalizedEmail)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                return await connection.QuerySingleOrDefaultAsync<IdentityUser>("Users_GetByEmail",
                    new { normalizedEmail }, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task<string> Create(IdentityUser user)
        {
            using (var connection = new SqlConnection(connetionString))
            {
                user.Id = Guid.NewGuid().ToString();
                await connection.ExecuteAsync("Users_Create", new
                {
                    user.Id,
                    user.Email,
                    user.NormalizedEmail,
                    user.UserName,
                    user.NormalizedUserName,
                    user.PasswordHash
                }, commandType: CommandType.StoredProcedure);
            }

            return user.Id;
        }

        public async Task<IList<Claim>> GetClaims(IdentityUser user)
        {
            using (var connetion = new SqlConnection(connetionString))
            {
                var claims = await connetion.QueryAsync<Claim>("Users_GetClaims", new { user.Id },
                    commandType: CommandType.StoredProcedure);
                return claims.ToList();
            }
        }

        public async Task AssignClaims(IdentityUser user, IEnumerable<Claim> claims)
        {
            var sql = @"INSERT INTO UsersClaims (UserId, ClaimType, ClaimValue) 
                                                VALUES (@Id, @Type, @Value)";
            var parameters = claims.Select(x => new {user.Id, x.Type, x.Value});

            using (var connetion = new SqlConnection(connetionString))
            {
                await connetion.ExecuteAsync(sql, parameters);
            }
        }

        public async Task RemoveClaims(IdentityUser user, IEnumerable<Claim> claims)
        {
            var sql = "DELETE UsersClaims WHERE UserId = @Id AND ClaimType = @Type";
            var parameters = claims.Select(x => new { user.Id, x.Type });

            using (var connetion = new SqlConnection(connetionString))
            {
                await connetion.ExecuteAsync(sql, parameters);
            }
        }
    }
}
