using Dapper;
using Microsoft.Data.SqlClient;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories.Interface;
using System.Data.Common;

namespace MoviesAPI_Minimal.Repostories
{
    public class GenreRepository : IGenreRepository
    {
        private readonly string connectionStrings;

        public GenreRepository(IConfiguration configuration)
        {
            connectionStrings = configuration.GetConnectionString("DefaultConnection")!;
        }
        public async Task<int> Create(Genre genres)
        {
            using (var connection = new SqlConnection(connectionStrings)) 
            {
                var query = @"
                                INSERT INTO Genres (Name) VALUES (@Name);

                                SELECT SCOPE_IDENTITY();
                            ";

                var id = await connection.QuerySingleAsync<int>(query, genres);
                genres.Id = id;

                return id;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionStrings))
            {
                var genres = await connection.QueryAsync<Genre>(@"SELECT Id, Name FROM Genres");

                return genres.ToList();
            }
            
        }

        public Task<Genre> GetById(int id)
        {
            using (var connnection = new SqlConnection(connectionStrings)) 
            { 
            
            
            }
        }
    }
}
