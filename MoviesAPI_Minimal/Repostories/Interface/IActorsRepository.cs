using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IActorsRepository
    {
        Task<int> Create(Actor actor);
        Task<List<Actor>> GetAll();
        Task<Actor?> GetById(int id);
        Task<bool> Exist(int id);
        Task Update(Actor actor);
        Task Delete(int id);
    }
}
