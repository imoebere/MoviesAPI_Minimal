﻿using AutoMapper;
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

            groupBuilder.MapPost("/", Create).DisableAntiforgery();
            return groupBuilder;

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