using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI_Minimal.Repostories.Interface;

namespace MoviesAPI_Minimal.DTOs
{
    public class CreateGenreRequestDTO
    {
        public IGenreRepository GenresRepository { get; set; } = null!;
        public IOutputCacheStore OutputCacheStore { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
