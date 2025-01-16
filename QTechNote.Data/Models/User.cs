using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QTechNote.Data.Models;

public class User
{
    public User()
    {
        AuthTokens = new HashSet<AuthToken>();
        Notes = new HashSet<Note>();
    }

    public int UserId { get; set; }

    [Required]
    [StringLength(50)]
    public string Username { get; set; } = null!;

    [Required]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    public bool IsActive { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AuthToken> AuthTokens { get; set; }
    public virtual ICollection<Note> Notes { get; set; }
}