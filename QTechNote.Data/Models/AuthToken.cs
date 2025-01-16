using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace QTechNote.Data.Models;

public class AuthToken
{
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public DateTime AccessTokenExpiry { get; set; }

    public DateTime RefreshTokenExpiry { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    [JsonIgnore]
    public virtual User User { get; set; } = null!;
}