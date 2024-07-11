using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IGenreRepository
    {
        Task<int> Create(Genre genres);
        Task<List<Genre>> GetAll();
        Task<Genre?> GetById(int id);
        Task<bool> Exists(int id);
        Task Update(Genre genre);
        Task Delete(int id);


    }
}
