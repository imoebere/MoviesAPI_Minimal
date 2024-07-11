using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.VisualBasic;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories;
using MoviesAPI_Minimal.Repostories.Interface;


var builder = WebApplication.CreateBuilder(args);

//Services Zone - BEGIN

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(configration =>
    {
        configration.WithOrigins(builder.Configuration["allowedOrigin"]!).AllowAnyMethod().AllowAnyHeader();
    });
    options.AddPolicy("free", configration =>
    {
        configration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddOutputCache();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IGenreRepository, GenreRepository>();
//Services Zone - END

var app = builder.Build();

// Middleware - BEGIN

/*
if (builder.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}*/

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", () => "Hello, World");

var genresEndpoints = app.MapGroup("/genres");

genresEndpoints.MapGet("/", GetGenres).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

genresEndpoints.MapGet("/{id:int}", GetById);

genresEndpoints.MapPost("/", Create);

genresEndpoints.MapPut("/{id:int}", Update);

genresEndpoints.MapDelete("/{id:int}", Delete);
// Middleware - END

app.Run();

static async Task<Ok<List<Genre>>> GetGenres (IGenreRepository genreRepository) 
{
    var genres = await genreRepository.GetAll();
    return TypedResults.Ok(genres);

}

static async Task<Results<Ok<Genre>, NotFound>> GetById (int id, IGenreRepository genreRepository) 
{
    var genres = await genreRepository.GetById(id);

    if (genres is null)
    {
        return TypedResults.NotFound();
    }
    return TypedResults.Ok(genres);
}

static async Task<Created<Genre>> Create (Genre genres, IGenreRepository genresRepository,
    IOutputCacheStore outputCacheStore) 
{
    await genresRepository.Create(genres);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.Created($"/genres/{genres.Id}", genres);
}

static async Task<Results<NotFound, NoContent>> Update (int id, Genre genres, IGenreRepository genreRepository,
    IOutputCacheStore outputCacheStore) 
{
    var exists = await genreRepository.Exists(id);

    if (!exists)
    {
        return TypedResults.NotFound();
    }

    await genreRepository.Update(genres);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.NoContent();
}

static async Task<Results<NoContent, NotFound>> Delete (int id, IGenreRepository genreRepository,
    IOutputCacheStore outputCacheStore)
{
    var exists = await genreRepository.Exists(id);

    if (!exists)
    {
        return TypedResults.NotFound();
    }
    await genreRepository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.NoContent();
}