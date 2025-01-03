using AutoMapper;
using MoviesAPI_Minimal.Repostories.Interface;

namespace MoviesAPI_Minimal.DTOs
{
    public class GetGenreByIdRequestDTO
    {
        public IGenreRepository Repository { get; set; } = null!;
        public int Id { get; set; }
        public IMapper Mapper { get; set; } = null!;
    }
}
