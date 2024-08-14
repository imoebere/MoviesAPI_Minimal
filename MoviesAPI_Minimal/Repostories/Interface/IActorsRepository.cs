using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task<List<Actor>> GetAll(PaginationDTO pagination);
        Task<Actor?> GetById(int id);
        Task<bool> Exist(int id);
        Task Update(Actor actor);
        Task Delete(int id);
        Task<List<Actor>> GetByName(string name);
        Task<List<int>> Exists(List<int> ids);
    }
}
