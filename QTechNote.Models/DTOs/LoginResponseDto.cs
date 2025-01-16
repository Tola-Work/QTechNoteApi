namespace QTechNote.Models.DTOs;

public class LoginResponseDto
{
    public int UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime AccessTokenExpiry { get; set; }
    public DateTime RefreshTokenExpiry { get; set; }
}

public class MenuDto
{
    public int MenuId { get; set; }
    public string MenuNameEn { get; set; } = string.Empty;
    public string? MenuNameKh { get; set; }
    public string? MenuPath { get; set; }
    public bool CanView { get; set; }
    public bool CanEdit { get; set; }
    public bool CanDelete { get; set; }
}