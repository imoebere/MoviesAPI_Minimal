using Dapper;
using Microsoft.Data.SqlClient;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories.Interface;
using System.Data;
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

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionStrings)) 
            {
                await connection.ExecuteAsync(@"DELETE Genres WHERE Id = @Id", new {id});
            }

        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(connectionStrings))
            {
                var exists = await connection.QuerySingleAsync<bool>(@"IF EXISTS 
                                                                        (SELECT 1 FROM Genres WHERE Id = @Id)
		                                                                    SELECT 1;
                                                                       ELSE
		                                                                     SELECT 0;", new {id});

                return exists;
            }
        }

        public async Task<List<Genre>> GetAll()
        {
            using (var connection = new SqlConnection(connectionStrings))
            {
                var genres = await connection.QueryAsync<Genre>(@"Genres_GetAll", 
                            commandType: CommandType.StoredProcedure);

                return genres.ToList();
            }
            
        }

        public async Task<Genre?> GetById(int id)
        {
            using (var connnection = new SqlConnection(connectionStrings)) 
            {
                var genres = await connnection.QueryFirstOrDefaultAsync<Genre>(@"SELECT Id, Name 
                                                                                FROM Genres
                                                                                WHERE Id = @Id", new { id });

                return genres;
            
            }
        }

        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(connectionStrings))
            {
                await connection.ExecuteAsync(@"UPDATE Genres 
                                                SET Name = @Name
                                                WHERE Id = @Id", genre);
            }
        }
    }
}
