using EmployeeManagement.Cadastro.Application.DTOs.Auth;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Domain.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace EmployeeManagement.Cadastro.API.Endpoints;

public static class AuthEndpoints
{
    public static WebApplication MapAuthEndpoints(this WebApplication app)
    {
        var auth = app.MapGroup("api/auth").WithTags("Auth");

        auth.MapPost("/register", async (UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService, IConfiguration configuration, RegisterDto dto) =>
        {
            var user = new ApplicationUser
            {
                UserName = dto.Email,
                Email = dto.Email,
                FullName = dto.FullName
            };

            var result = await userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
                return Results.BadRequest(result.Errors);

            var token = jwtTokenService.GenerateToken(user);
            var refreshToken = jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(configuration["JwtSettings:RefreshTokenExpirationInDays"] ?? "7"));
            await userManager.UpdateAsync(user);

            return Results.Created($"/api/auth/user/{user.Id}", new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60")),
                Email = user.Email ?? string.Empty,
                FullName = user.FullName
            });
        });

        auth.MapPost("/login", async (SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService, IConfiguration configuration, LoginDto dto) =>
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Results.Unauthorized();

            var result = await signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
                return Results.Unauthorized();

            var token = jwtTokenService.GenerateToken(user);
            var refreshToken = jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(configuration["JwtSettings:RefreshTokenExpirationInDays"] ?? "7"));
            await userManager.UpdateAsync(user);

            return Results.Ok(new AuthResponseDto
            {
                Token = token,
                RefreshToken = refreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60")),
                Email = user.Email ?? string.Empty,
                FullName = user.FullName
            });
        });

    auth.MapPost("/refresh", async (UserManager<ApplicationUser> userManager, IJwtTokenService jwtTokenService, IConfiguration configuration, RefreshTokenDto dto) =>
        {
            var principal = jwtTokenService.ValidateToken(dto.Token);
            if (principal == null)
                return Results.Unauthorized();

            var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var user = await userManager.FindByIdAsync(userId ?? string.Empty);

            if (user == null || user.RefreshToken != dto.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
                return Results.Unauthorized();

            var newToken = jwtTokenService.GenerateToken(user);
            var newRefreshToken = jwtTokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(int.Parse(configuration["JwtSettings:RefreshTokenExpirationInDays"] ?? "7"));
            await userManager.UpdateAsync(user);

            return Results.Ok(new AuthResponseDto
            {
                Token = newToken,
                RefreshToken = newRefreshToken,
                TokenExpiration = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JwtSettings:ExpirationInMinutes"] ?? "60")),
                Email = user.Email ?? string.Empty,
                FullName = user.FullName
            });
        });

        return app;
    }
}
