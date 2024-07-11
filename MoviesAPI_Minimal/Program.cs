using Microsoft.AspNetCore.Cors;
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

app.MapGet("/genres", async (IGenreRepository genreRepository) =>
{
    return await genreRepository.GetAll();

}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));

app.MapGet("/genres/{id:int}", async (int id, IGenreRepository genreRepository) =>
{
    var genres = await genreRepository.GetById(id);
    
    if(genres is null)
    {
        return Results.NotFound();
    }
    return Results.Ok(genres);
});

app.MapPost("/genres", async (Genre genres, IGenreRepository genresRepository, 
    IOutputCacheStore outputCacheStore) =>
{
    await genresRepository.Create(genres);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return TypedResults.Created($"/genres/{genres.Id}", genres);
});

app.MapPut("/genres/{id:int}", async (int id, Genre genres, IGenreRepository genreRepository,
    IOutputCacheStore outputCacheStore) =>
{
    var exists = await genreRepository.Exists(id);

    if (!exists)
    {
        return Results.NotFound();
    }

    await genreRepository.Update(genres);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});

app.MapDelete("/genres/{id:int}", async (int id, IGenreRepository genreRepository,
    IOutputCacheStore outputCacheStore) =>
{
    var exists = await genreRepository.Exists(id);

    if (!exists)
    {
        return Results.NotFound();
    }
    await genreRepository.Delete(id);
    await outputCacheStore.EvictByTagAsync("genres-get", default);
    return Results.NoContent();
});
// Middleware - END

app.Run();
