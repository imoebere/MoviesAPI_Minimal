﻿using Dapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using MoviesAPI_Minimal.Repostories.Interface;
using System.Data;

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
    }
}