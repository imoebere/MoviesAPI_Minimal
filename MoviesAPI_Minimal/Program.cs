using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MoviesAPI_Minimal.Endpoints;
using MoviesAPI_Minimal.Entities;
using MoviesAPI_Minimal.Repostories;
using MoviesAPI_Minimal.Repostories.Interface;
using MoviesAPI_Minimal.Services;
using MoviesAPI_Minimal.Services.Interface;
using MoviesAPI_Minimal.Utilities;


var builder = WebApplication.CreateBuilder(args);

//Services Zone - BEGIN

builder.Services.AddTransient<IUserStore<IdentityUser>, UserStore>();
builder.Services.AddIdentityCore<IdentityUser>();
builder.Services.AddTransient<SignInManager<IdentityUser>>();

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

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Movies API",
        Description = "This is a Web API for Movies", 
        Contact = new OpenApiContact
        {
            Email = "imoebereedward2017@gmail.com",
            Name = "Ebere Imo", 
            Url = new Uri ("https://github.com/imoebere/MoviesAPI_Minimal")
        }
    });
});

builder.Services.AddScoped<IGenreRepository, GenreRepository>();
builder.Services.AddScoped<IActorsRepository, ActorsRepository>();
builder.Services.AddScoped<IMoviesRepository, MoviesRepository>();
builder.Services.AddScoped<ICommentsRepository, CommentsRepository>();
builder.Services.AddScoped<IErrorRepository, ErrorRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddTransient<IFileStorage, LocalFileStorage>();
builder.Services.AddTransient<IUsersService, UsersService>();
builder.Services.AddHttpContextAccessor();


builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services.AddProblemDetails();

builder.Services.AddAuthentication().AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ClockSkew = TimeSpan.Zero,
        IssuerSigningKeys = KeysHandler.GetAllKeys(builder.Configuration),
        //IssuerSigningKey = KeysHandler.GetKey(builder.Configuration).First()
    };
});
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("isadmin", policy => policy.RequireClaim("isadmin"));
});

//Services Zone - END

var app = builder.Build();

// Middleware - BEGIN


app.UseSwagger();
app.UseSwaggerUI();

app.UseExceptionHandler(exceptionHandlerApp => exceptionHandlerApp.Run(async context =>
{
    var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
    var exception = exceptionHandlerFeature?.Error!;

    var error = new Error();
    error.Date = DateTime.UtcNow;
    error.ErrorMessage = exception.Message;
    error.StackTrace = exception.StackTrace;

    var repository = context.RequestServices.GetRequiredService<IErrorRepository>();
    await repository.Create(error);

    await Results
    .BadRequest(new 
        { type = "error", 
          message = "An unexpected exception has occured", 
          status = 500 
        }).ExecuteAsync(context);
}));
app.UseStatusCodePages();

app.UseStaticFiles();

app.UseCors();

app.UseOutputCache();
app.UseAuthorization();

//app.MapGet("/", () => "Hello, World");
app.MapGet("/error", () =>
{
    throw new InvalidOperationException("example error");
});

/*app.MapPost("/modelbinding", ([FromHeader] string? name) =>
{
    if (name is null) name = "Empty";
    return TypedResults.Ok(name);

});*/

app.MapGroup("/genres").MapGenres();
app.MapGroup("/actors").MapActors();
app.MapGroup("/movies").MapMovies();
app.MapGroup("/movies/{movieId:int}/comments").MapComments();
app.MapGroup("/users").MapUsers();

// Middleware - END

app.Run();

