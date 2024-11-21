using FluentValidation;
using MoviesAPI_Minimal.Endpoints;
using MoviesAPI_Minimal.Repostories;
using MoviesAPI_Minimal.Repostories.Interface;
using MoviesAPI_Minimal.Services;


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
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

//Services Zone - END

var app = builder.Build();

// Middleware - BEGIN


app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();

//app.MapGet("/", () => "Hello, World");

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movies/{movieId:int}/comments").MapComments();

// Middleware - END

app.Run();

