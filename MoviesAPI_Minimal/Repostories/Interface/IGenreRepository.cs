using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IGenreRepository
    {
        Task<int> Create(Genre genres);
        Task<List<Genre>> GetAll();
        Task<Genre> GetById(int id);


    }
}
