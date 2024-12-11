using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Filters;
using MoviesAPI_Minimal.Repostories.Interface;
using MoviesAPI_Minimal.Services;
using MoviesAPI_Minimal.Services.Interface;

namespace MoviesAPI_Minimal.Endpoints
{
    public static class CommentsEndpoints
    {
        public static RouteGroupBuilder MapComments(this RouteGroupBuilder group) 
        {
            group.MapGet("/", GetAll)
                .CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("comments-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>()
                .RequireAuthorization();

            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateCommentDTO>>()
                .RequireAuthorization();
            group.MapDelete("/{id:int}", Delete).RequireAuthorization();
            return group;
        }
        static async Task<Results<Ok<List<CommentDTO>>, NotFound>> GetAll(int movieId, ICommentsRepository commentsRepository, 
            IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exist(movieId))
            {
               return TypedResults.NotFound();
            }

            var comment = await commentsRepository.GetAll(movieId);
            var commentDTO = mapper.Map<List<CommentDTO>>(comment);
            return TypedResults.Ok(commentDTO);
        }

        static async Task<Results<Ok<CommentDTO>, NotFound>> GetById(int movieId, int Id, 
            ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper)
        {
            if (!await moviesRepository.Exist(movieId))
            {
                return TypedResults.NotFound();
            }

            var comment = await commentsRepository.GetById(Id);

            if (comment == null) 
            {
                return TypedResults.NotFound();
            }

            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Ok(commentDTO);
        }
        static async Task<Results<Created<CommentDTO>, NotFound, BadRequest<string>>> Create(int movieId, 
            CreateCommentDTO createCommentDTO, ICommentsRepository commentsRepository, 
            IMoviesRepository moviesRepository, IOutputCacheStore outputCacheStore, IMapper mapper,
            IUsersService usersService)
        {
            if (!await moviesRepository.Exist(movieId))
            { 
                return TypedResults.NotFound();
            }

            var user = await usersService.GetUser();

            if (user is null)
            {
                return TypedResults.BadRequest("User not found");
            }

            var comment = mapper.Map<Comment>(createCommentDTO);
            comment.MovieId = movieId;
            comment.UserId = user.Id;
            var id = await commentsRepository.Create(comment);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            var commentDTO = mapper.Map<CommentDTO>(comment);
            return TypedResults.Created($"/comment/{id}", commentDTO);
        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Update(int movieId, int Id,
            CreateCommentDTO createCommentDTO, IOutputCacheStore outputCacheStore, 
            ICommentsRepository commentsRepository, IMoviesRepository moviesRepository,IMapper mapper, 
            IUsersService usersService)
        {
            if (!await moviesRepository.Exist(movieId)) 
            { 
                return TypedResults.NotFound();
            }
            var commentFromDB = await commentsRepository.GetById(Id);

            if (commentFromDB is null) 
            {
                return TypedResults.NotFound();
            }

            var user = await usersService.GetUser();

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            if(commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }

            commentFromDB.Body = createCommentDTO.Body;
            
            await commentsRepository.Update(commentFromDB);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();

        }

        static async Task<Results<NoContent, NotFound, ForbidHttpResult>> Delete(int movieId, int Id, IOutputCacheStore outputCacheStore,
            ICommentsRepository commentsRepository, IMoviesRepository moviesRepository, IMapper mapper,
            IUsersService usersService)
        {
            if (!await moviesRepository.Exist(movieId))
            {
                return TypedResults.NotFound();
            }

            /* if (!await commentsRepository.Exist(Id))
             {
                 return TypedResults.NotFound();
             }*/

            var commentFromDB = await commentsRepository.GetById(Id);

            if (commentFromDB is null) 
            {
                return TypedResults.NotFound();
            }

            var user = await usersService.GetUser();

            if (user is null)
            {
                return TypedResults.NotFound();
            }

            if (commentFromDB.UserId != user.Id)
            {
                return TypedResults.Forbid();
            }

            
            await commentsRepository.Delete(Id);
            await outputCacheStore.EvictByTagAsync("comments-get", default);
            return TypedResults.NoContent();
        }

    }
}
