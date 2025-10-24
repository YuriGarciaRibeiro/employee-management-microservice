using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using EmployeeManagement.Cadastro.Domain.Entities;
using EmployeeManagement.Cadastro.Infrastructure.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace EmployeeManagement.Cadastro.Application.Tests.Services;

public class JwtTokenServiceTests
{
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly JwtTokenService _jwtTokenService;

    public JwtTokenServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        SetupConfiguration();
        _jwtTokenService = new JwtTokenService(_configurationMock.Object);
    }

    private void SetupConfiguration()
    {
        _configurationMock.Setup(x => x["JwtSettings:Secret"]).Returns("ThisIsAVerySecretKeyForJwtTokenGenerationWithAtLeast32Characters");
        _configurationMock.Setup(x => x["JwtSettings:Issuer"]).Returns("EmployeeManagement");
        _configurationMock.Setup(x => x["JwtSettings:Audience"]).Returns("EmployeeManagementUsers");
        _configurationMock.Setup(x => x["JwtSettings:ExpirationInMinutes"]).Returns("60");
    }

    [Fact]
    public void GenerateToken_ValidUser_ShouldReturnToken()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@empresa.com",
            FullName = "Test User",
            UserName = "testuser"
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();
        token.Should().NotContain(" ");

        var handler = new JwtSecurityTokenHandler();
        handler.CanReadToken(token).Should().BeTrue();
    }

    [Fact]
    public void GenerateToken_ShouldContainUserClaims()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "joao.silva@empresa.com",
            FullName = "João Silva",
            UserName = "joaosilva"
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Sub && c.Value == user.Id);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == user.Email);
        jwtToken.Claims.Should().Contain(c => c.Type == "fullName" && c.Value == user.FullName);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Jti);
    }

    [Fact]
    public void GenerateToken_ShouldHaveCorrectIssuerAndAudience()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@empresa.com",
            FullName = "Test User",
            UserName = "testuser"
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.Issuer.Should().Be("EmployeeManagement");
        jwtToken.Audiences.Should().Contain("EmployeeManagementUsers");
    }

    [Fact]
    public void GenerateToken_ShouldHaveExpirationTime()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@empresa.com",
            FullName = "Test User",
            UserName = "testuser"
        };

        var beforeGeneration = DateTime.UtcNow;

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        var afterGeneration = DateTime.UtcNow;

        // Assert
        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        jwtToken.ValidTo.Should().BeAfter(beforeGeneration.AddMinutes(59));
        jwtToken.ValidTo.Should().BeBefore(afterGeneration.AddMinutes(61));
    }

    [Fact]
    public void GenerateRefreshToken_ShouldReturnNonEmptyString()
    {
        // Act
        var refreshToken = _jwtTokenService.GenerateRefreshToken();

        // Assert
        refreshToken.Should().NotBeNullOrEmpty();
        refreshToken.Length.Should().BeGreaterThan(20);
    }

    [Fact]
    public void GenerateRefreshToken_ShouldGenerateDifferentTokensEachTime()
    {
        // Act
        var token1 = _jwtTokenService.GenerateRefreshToken();
        var token2 = _jwtTokenService.GenerateRefreshToken();
        var token3 = _jwtTokenService.GenerateRefreshToken();

        // Assert
        token1.Should().NotBe(token2);
        token2.Should().NotBe(token3);
        token1.Should().NotBe(token3);
    }

    [Fact]
    public void ValidateToken_ValidToken_ShouldReturnClaimsPrincipal()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@empresa.com",
            FullName = "Test User",
            UserName = "testuser"
        };

        var token = _jwtTokenService.GenerateToken(user);

        // Act
        var principal = _jwtTokenService.ValidateToken(token);

        // Assert
        principal.Should().NotBeNull();
        // JWT claims are mapped to standard claim types
        principal.Claims.Should().Contain(c => c.Value == user.Id);
        principal.Claims.Should().Contain(c => c.Value == user.Email);
        principal.Claims.Should().Contain(c => c.Type == "fullName" && c.Value == user.FullName);
    }

    [Theory]
    [InlineData("")]
    [InlineData("invalid.token.here")]
    [InlineData("eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c")]
    public void ValidateToken_InvalidToken_ShouldReturnNull(string invalidToken)
    {
        // Act
        var principal = _jwtTokenService.ValidateToken(invalidToken);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void ValidateToken_TokenFromDifferentSecret_ShouldReturnNull()
    {
        // Arrange
        var differentConfigMock = new Mock<IConfiguration>();
        differentConfigMock.Setup(x => x["JwtSettings:Secret"]).Returns("DifferentSecretKeyThatIsAtLeast32CharactersLong12345");
        differentConfigMock.Setup(x => x["JwtSettings:Issuer"]).Returns("EmployeeManagement");
        differentConfigMock.Setup(x => x["JwtSettings:Audience"]).Returns("EmployeeManagementUsers");
        differentConfigMock.Setup(x => x["JwtSettings:ExpirationInMinutes"]).Returns("60");

        var differentService = new JwtTokenService(differentConfigMock.Object);

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = "test@empresa.com",
            FullName = "Test User",
            UserName = "testuser"
        };

        var token = differentService.GenerateToken(user);

        // Act
        var principal = _jwtTokenService.ValidateToken(token);

        // Assert
        principal.Should().BeNull();
    }

    [Fact]
    public void GenerateToken_WithNullEmail_ShouldGenerateTokenWithEmptyEmail()
    {
        // Arrange
        var user = new ApplicationUser
        {
            Id = Guid.NewGuid().ToString(),
            Email = null,
            FullName = "Test User",
            UserName = "testuser"
        };

        // Act
        var token = _jwtTokenService.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);
        jwtToken.Claims.Should().Contain(c => c.Type == JwtRegisteredClaimNames.Email && c.Value == string.Empty);
    }
}
