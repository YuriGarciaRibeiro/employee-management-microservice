using System.Security.Claims;
using EmployeeManagement.Cadastro.Domain.Entities;

namespace EmployeeManagement.Cadastro.Domain.Interfaces;

public interface IJwtTokenService
{
    string GenerateToken(ApplicationUser user);
    string GenerateRefreshToken();
    ClaimsPrincipal? ValidateToken(string token);
}