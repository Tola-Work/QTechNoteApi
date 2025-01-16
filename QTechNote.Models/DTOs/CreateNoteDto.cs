using System.ComponentModel.DataAnnotations;

namespace QTechNote.Models.DTOs;

public class CreateNoteDto
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}