using MoviesAPI_Minimal.Endpoints;
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
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();

builder.Services.AddAutoMapper(typeof(Program));
//Services Zone - END

var app = builder.Build();

// Middleware - BEGIN


app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", () => "Hello, World");

app.MapGroup("/genres").MapGenres();

// Middleware - END

app.Run();

