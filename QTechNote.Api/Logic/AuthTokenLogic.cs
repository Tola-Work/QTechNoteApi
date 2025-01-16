using QTechNote.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace QTechNote.Api.Logic;

public class AuthTokenLogic : BaseLogic<AuthToken>
{
    public AuthTokenLogic(QTechNoteContext context) : base(context) { }

    public async Task<bool> ValidateTokenAsync(string token)
    {
        var authToken = await _dbSet.FirstOrDefaultAsync(t =>
            t.AccessToken == token || t.RefreshToken == token);

        if (authToken == null) return false;

        // Check if it's an access token or refresh token and validate accordingly
        if (token == authToken.AccessToken)
            return (authToken.AccessTokenExpiry > DateTime.UtcNow) && (authToken.IsActive ?? false);
        else
            return (authToken.RefreshTokenExpiry > DateTime.UtcNow) && (authToken.IsActive ?? false);
    }
}
