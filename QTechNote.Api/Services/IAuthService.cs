using QTechNote.Data.Models;
using QTechNote.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace QTechNote.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponseDto> Login(LoginRequestDto request);
        Task<LoginResponseDto> RefreshToken(RefreshTokenRequestDto request);
        Task Logout(int userId);
        bool VerifyPasswordHash(string password, byte[] storedHash, byte[] storedSalt);
    }
}