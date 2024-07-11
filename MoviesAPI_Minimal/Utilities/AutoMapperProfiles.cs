using AutoMapper;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;

namespace MoviesAPI_Minimal.Utilities
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles() 
        {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();
        }
    }
}
