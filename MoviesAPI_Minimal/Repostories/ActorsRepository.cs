using Dapper;
using Microsoft.Data.SqlClient;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories.Interface;
using System.Data.Common;
using System.Data;

namespace MoviesAPI_Minimal.Repostories
{
    public class ActorsRepository : IActorsRepository
    {
        private readonly string connectionString;

        public ActorsRepository(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection")!;
        }

        public async Task<int> Create(Actor actor)
        {
            using (var connection = new SqlConnection(connectionString)) 
            {
                var id = await connection.QuerySingleAsync<int>("Actors_Create", new
                {
                    actor.Name,
                    actor.DateOfBirth,
                    actor.Picture
                }, commandType: CommandType.StoredProcedure);
                
                actor.Id = id;
                return id;
            }
        }

        public async Task<List<Actor>> GetAll()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var actor = await connection.QueryAsync<Actor>("Actors_GetAll",
                    commandType: CommandType.StoredProcedure);

                return actor.ToList();
            }
        }

        public async Task<Actor?> GetById(int id)
        {
            using (var connection = new SqlConnection(connectionString)) 
            { 
                var actor = await connection.QueryFirstOrDefaultAsync<Actor>("Actors_GetById", 
                    new { id }, commandType: CommandType.StoredProcedure);

                return actor;
            }
        }

        public async Task<bool> Exist(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                var exist = await connection.QuerySingleAsync<bool>("Actors_Exist", new {id}, 
                    commandType: CommandType.StoredProcedure);

                return exist;
            }
        }  

        public async Task Update(Actor actor)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Actors_Update", new
                {
                    actor.Id,
                    actor.Name,
                    actor.DateOfBirth,
                    actor.Picture
                }, commandType: CommandType.StoredProcedure);

            }
        }

        public async Task Delete(int id)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.ExecuteAsync("Actors_Delete", new {id},
                    commandType: CommandType.StoredProcedure);  
            }
        }
    }
}
