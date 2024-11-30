using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Repostories.Interface
{
    public interface IErrorRepository
    {
        Task<Guid> Create(Error error);
    }
}