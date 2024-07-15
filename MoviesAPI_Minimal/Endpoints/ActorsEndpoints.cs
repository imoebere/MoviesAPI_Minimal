using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories.Interface;
using MoviesAPI_Minimal.Services;

namespace MoviesAPI_Minimal.Endpoints
{
    public static class ActorsEndpoints
    {
        private readonly static string container = "actors";
        public static RouteGroupBuilder MapActors(this RouteGroupBuilder groupBuilder) 
        {
            groupBuilder.MapGet("/", GetAll)
                .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("actors-get"));
            groupBuilder.MapGet("/{id:int}", GetById);
            groupBuilder.MapGet("getByName/{name}", GetByName);
            groupBuilder.MapPost("/", Create).DisableAntiforgery();
            return groupBuilder;

        }

        static async Task<Ok<List<ActorDTO>>> GetAll(IActorsRepository actorsRepository,
            IMapper mapper, int page = 1, int recordsPerPage = 10 )
        {
            var pagination = new PaginationDTO { Page = page, RecordsPerPage = recordsPerPage};
           var actors =  await actorsRepository.GetAll(pagination);
            var actorsDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDTO);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> GetById(int id, IActorsRepository actorsRepository,
            IMapper mapper)
        {
            var actor = await actorsRepository.GetById(id);

            if (actor is null) 
            {
                return TypedResults.NotFound();
            }

            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Ok<List<ActorDTO>>> GetByName(string name, IActorsRepository actorsRepository, IMapper mapper)
        {
            var actors = await actorsRepository.GetByName(name);
            var actorsDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDTO);
        }
        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO createActorDTO, 
            IActorsRepository actorsRepository, IOutputCacheStore outputCacheStore, IMapper mapper, 
            IFileStorage fileStorage)
        {
            var actors = mapper.Map<Actor>(createActorDTO);

            if (createActorDTO.Picture is not null) 
            {
                var url = await fileStorage.Store(container, createActorDTO.Picture);
                actors.Picture = url;
            }
            var id = await actorsRepository.Create(actors);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            var actorsDTO = mapper.Map<ActorDTO>(actors);
            return TypedResults.Created($"/actors/{id}", actorsDTO);
        }
    }
}
