using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IMoviesRepository
    {
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exist(int id);
        Task<List<Movie>> GetAll(PaginationDTO pagination);
        Task<Movie?> GetById(int id);
        Task Update(Movie movie);
    }
}