using Microsoft.AspNetCore.Cors;
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

app.MapGet("/genres", () =>
{
    var genres = new List<Genre>()
    {
        new Genre
        {
            Id = 1,
            Name = "Drama"
        },
        new Genre
        {
            Id = 2,
            Name = "Action"
        },
        new Genre
        {
            Id = 3,
            Name = "Comedy"
        }
    };

    return genres;
}).CacheOutput(c => c.Expire(TimeSpan.FromSeconds(15)));


app.MapPost("/genres", async (Genre genres, IGenreRepository genresRepository) =>
{
    await genresRepository.Create(genres);
    return TypedResults.Created($"/genres/{genres.Id}", genres);
});
// Middleware - END

app.Run();
