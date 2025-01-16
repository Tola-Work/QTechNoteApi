using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using QTechNote.Data.Models;

namespace QTechNote.Data.Models;

public partial class Note
{
    [Key]
    public int NoteId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = null!;

    [Required]
    public string Content { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    [Required]
    public int UserId { get; set; }

    [JsonIgnore]
    public virtual User? User { get; set; }
}