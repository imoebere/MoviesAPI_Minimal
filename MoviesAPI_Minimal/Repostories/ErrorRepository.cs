using Dapper;
using Microsoft.Data.SqlClient;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories.Interface;
using System.Data;

namespace MoviesAPI_Minimal.Repostories
{
    public class ErrorRepository : IErrorRepository
    {
        private readonly string connectionString;
        public ErrorRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<Guid> Create(Error error)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                error.Id = Guid.NewGuid();

                await connection.ExecuteAsync("Errors_Create", new
                {
                    error.Id,
                    error.ErrorMessage,
                    error.StackTrace,
                    error.Date
                }, commandType: CommandType.StoredProcedure);
                return error.Id;
            }
        }

    }
}
