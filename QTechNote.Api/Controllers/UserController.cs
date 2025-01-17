using QTechNote.Api.Logic;
using QTechNote.Api.Services;
using QTechNote.Data.Models;
using QTechNote.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace QTechNote.Api.Controllers;

public class UserController : BaseController<User, UserLogic>
{
    private readonly IAuthService _authService;

    public UserController(UserLogic logic, IAuthService authService) : base(logic)
    {
        _authService = authService;
    }

    protected override int GetEntityId(User entity) => entity.UserId;

    [AllowAnonymous]
    [HttpPost]
    public async Task<ActionResult<LoginResponseDto>> Register(RegisterRequestDto request)
    {
        // Move this log UserLogic.cs
        if (await _logic.UsernameExistsAsync(request.Email))
        {
            return BadRequest("Email already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow,
            IsActive = true,
            // Don't set PasswordHash and PasswordSalt here
            // They will be set in UserLogic.CreateAsync
        };

        // The password string is passed through the PasswordHash property temporarily
        // and will be properly hashed in UserLogic.CreateAsync
        using (var hmac = new System.Security.Cryptography.HMACSHA512())
        {
            user.PasswordSalt = hmac.Key;
            user.PasswordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(request.Password));
        }

        var createdUser = await _logic.CreateAsync(user);

        try
        {
            var loginResponse = await _authService.Login(new LoginRequestDto
            {
                Email = request.Email,
                Password = request.Password
            });

            return Ok(loginResponse);
        }
        catch (Exception)
        {
            // Return an error response
            return StatusCode(500, new { Message = "An error occurred while creating the user. Please try again later." });
        }

    }
}
