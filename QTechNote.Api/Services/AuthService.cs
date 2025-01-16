using Microsoft.EntityFrameworkCore;
using QTechNote.Data.Models;
using QTechNote.Models.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace QTechNote.Api.Services;

public class AuthService : IAuthService
{
    private readonly QTechNoteContext _context;
    private readonly IConfiguration _configuration;

    public AuthService(QTechNoteContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto request)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username && u.IsActive == true);

        if (user == null)
            throw new UnauthorizedAccessException("Invalid username or password");

        if (!VerifyPasswordHash(request.Password, user.PasswordHash, user.PasswordSalt))
            throw new UnauthorizedAccessException("Invalid username or password");

        return await CreateToken(user);
    }

    public async Task<LoginResponseDto> RefreshToken(RefreshTokenRequestDto request)
    {
        var refreshToken = await _context.AuthTokens
            .Include(t => t.User)
            .FirstOrDefaultAsync(t =>
                t.RefreshToken == request.RefreshToken &&
                t.IsActive == true);

        if (refreshToken == null)
            throw new UnauthorizedAccessException("Invalid refresh token");

        if (refreshToken.RefreshTokenExpiry < DateTime.UtcNow)
        {
            refreshToken.IsActive = false;
            await _context.SaveChangesAsync();
            throw new UnauthorizedAccessException("Refresh token expired");
        }

        // Invalidate the current refresh token
        refreshToken.IsActive = false;
        await _context.SaveChangesAsync();

        // Create new tokens
        return await CreateToken(refreshToken.User);
    }

    public async Task Logout(int userId)
    {
        var tokens = await _context.AuthTokens
            .Where(t => t.UserId == userId && t.IsActive == true)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.IsActive = false;
        }

        await _context.SaveChangesAsync();
    }

    public bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt)
    {
        using var hmac = new System.Security.Cryptography.HMACSHA512(storedSalt);
        var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        return computedHash.SequenceEqual(storedHash);
    }

    private async Task<LoginResponseDto> CreateToken(User user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email ?? "")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
            _configuration.GetSection("AppSettings:Token").Value!));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: creds);

        var jwt = new JwtSecurityTokenHandler().WriteToken(token);
        var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        var authToken = new AuthToken
        {
            UserId = user.UserId,
            AccessToken = jwt,
            RefreshToken = refreshToken,
            AccessTokenExpiry = DateTime.UtcNow.AddDays(1),
            RefreshTokenExpiry = DateTime.UtcNow.AddDays(5),
            IsActive = true
        };

        _context.AuthTokens.Add(authToken);
        await _context.SaveChangesAsync();

        return new LoginResponseDto
        {
            UserId = user.UserId,
            Email = user.Email,
            AccessToken = jwt,
            RefreshToken = refreshToken,
            AccessTokenExpiry = authToken.AccessTokenExpiry,
            RefreshTokenExpiry = authToken.RefreshTokenExpiry
        };
    }
}