using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QTechNote.Data.Models;
using QTechNote.Models.DTOs;
using System.Security.Cryptography;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using QTechNote.Api.Services;

namespace QTechNote.Api.Controllers;
[ApiController]
[Route("api/[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginRequestDto request)
    {
        try
        {
            var response = await _authService.Login(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponseDto>> RefreshToken(RefreshTokenRequestDto request)
    {
        try
        {
            var response = await _authService.RefreshToken(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userId == null)
            return Unauthorized();

        await _authService.Logout(int.Parse(userId));
        return Ok();
    }
}