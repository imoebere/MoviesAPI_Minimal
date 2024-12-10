using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MoviesAPI_Minimal.DTOs;
using MoviesAPI_Minimal.Filters;
using MoviesAPI_Minimal.Utilities;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using System.Security.Claims;

namespace MoviesAPI_Minimal.Endpoints
{
    public static class UsersEndpoints
    {
        public static RouteGroupBuilder MapUsers(this RouteGroupBuilder group) 
        {
            group.MapPost("/register", Register).AddEndpointFilter<ValidationFilter<UserCredentialsDTO>>();
            group.MapPost("/Login", Login).AddEndpointFilter<ValidationFilter<UserCredentialsDTO>>();
            return group;
        }

        static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<IEnumerable<IdentityError>>>> Register(
            UserCredentialsDTO userCredentialsDTO, [FromServices] UserManager<IdentityUser> userManager, 
            IConfiguration configuration)
        {
            var user = new IdentityUser
            {
                UserName = userCredentialsDTO.Email,
                Email = userCredentialsDTO.Email
            };

            var result = await userManager.CreateAsync(user, userCredentialsDTO.Password);

            if (result.Succeeded)
            {
                var authenticationResponse = await BuildToken(userCredentialsDTO, configuration, userManager);
                return TypedResults.Ok(authenticationResponse);
            }
            else
            {
                return TypedResults.BadRequest(result.Errors);
            }
        }
        static async Task<Results<Ok<AuthenticationResponseDTO>, BadRequest<string>>> Login(
            UserCredentialsDTO userCredentialsDTO, [FromServices] SignInManager<IdentityUser> signInManager, 
            [FromServices] UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            var user = await userManager.FindByEmailAsync(userCredentialsDTO.Email);
            if (user == null) 
            {
                return TypedResults.BadRequest("There was a problem with the Email or Password");
            }
            var results = await signInManager.CheckPasswordSignInAsync(user, 
                userCredentialsDTO.Password, lockoutOnFailure: false);

            if (results.Succeeded) 
            {
                var authenticationResponse = await BuildToken(userCredentialsDTO, configuration, userManager);
                return TypedResults.Ok(authenticationResponse);
            }
            else
            {
                return TypedResults.BadRequest("There was a problem with the Email or Password");
            }

        }

        private async static Task<AuthenticationResponseDTO> BuildToken(UserCredentialsDTO userCredentialsDTO,
            IConfiguration configuration, UserManager<IdentityUser> userManager)
        {

            /*var user = await userManager.FindByNameAsync(userCredentialsDTO.Email);
            if (user == null) throw new InvalidOperationException("User not found while building token.");*/

            var claims = new List<Claim>
            {
                new Claim("email", userCredentialsDTO.Email)
                //new Claim("Whatever I want", "This is a value")
            };
            /*var user = await userManager.FindByNameAsync(userCredentialsDTO.Email);
            var claimsFromDB = await userManager.GetClaimsAsync(user!);
            claims.AddRange(claimsFromDB);*/

            var key = KeysHandler.GetKey(configuration).First();
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = DateTime.UtcNow.AddYears(1);

            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,
                expires: expiration, signingCredentials: credentials);

            var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

            return new AuthenticationResponseDTO
            {
                Token = token,
                Expiration = expiration
            };
        }
    }
}
