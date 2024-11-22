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
                //var query = "Genres_Create";

                var id = await connection.QuerySingleAsync<int>("Genres_Create", new { genres.Name },
                    commandType: CommandType.StoredProcedure);
                genres.Id = id;

                return id;
            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionStrings)) 
            {
                await connection.ExecuteAsync("Genres_Delete", new {id}, 
                    commandType: CommandType.StoredProcedure);
            }

        }

        public async Task<bool> Exists(int id)
        {
            using (var connection = new SqlConnection(connectionStrings))
            {
                var exists = await connection.QuerySingleAsync<bool>("Genres_Exists", new {id}, 
                    commandType: CommandType.StoredProcedure);

                return exists;
            }
        }

        public async Task<bool> Exists(int id, string name)
        {
            using(var connection = new SqlConnection(connectionStrings))
            {
                var exists = await connection.QuerySingleAsync<bool>("Genres_ExistsByIdAndName", new {id, name},
                    commandType: CommandType.StoredProcedure);

                return exists;
            }
        }

        public async Task<List<int>> Exists(List<int> Ids)
        {
            var dt = new DataTable();
            dt.Columns.Add("Id", typeof(int));

            foreach (var genresId in Ids)
            {
                dt.Rows.Add(genresId);
            }

            using (var connection = new SqlConnection(connectionStrings))
            {
                var idsOfGenresThatExists = await connection.QueryAsync<int>("Genres_GetBySeveralIds",
                    new { genresIds = dt }, commandType: CommandType.StoredProcedure);

                return idsOfGenresThatExists.ToList();
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
                var genres = await connnection.QueryFirstOrDefaultAsync<Genre>("Genres_GetById", new { id },
                  commandType: CommandType.StoredProcedure);

                return genres;
            
            }
        }

        public async Task Update(Genre genre)
        {
            using (var connection = new SqlConnection(connectionStrings))
            {
                await connection.ExecuteAsync("Genres_Update", new { genre.Id, genre.Name }, 
                    commandType: CommandType.StoredProcedure);
            }
        }
    }
}
