using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QTechNote.Api.Logic;
using QTechNote.Data.Models;
using System.Security.Claims;
using QTechNote.Models.DTOs;

namespace QTechNote.Api.Controllers;

public class NoteController : BaseController<Note, NoteLogic>
{
    public NoteController(NoteLogic logic) : base(logic) { }

    protected override int GetEntityId(Note entity) => entity.NoteId;

    [HttpGet]
    public override async Task<ActionResult<PaginationResponseDto<Note>>> GetAll(PaginationDto pagination)
    {
        var userId = GetCurrentUserId();
        var notes = await _logic.GetUserNotesAsync(userId, pagination);
        return Ok(notes);
    }

    [HttpGet("{id}")]
    public override async Task<ActionResult<Note>> GetById(int id)
    {
        var userId = GetCurrentUserId();
        var note = await _logic.GetNoteByIdAsync(id, userId);

        if (note == null)
            return NotFound();

        return Ok(note);
    }

    [HttpPost]
    public override async Task<ActionResult<Note>> Create([FromBody] Note note)
    {
        try
        {
            var userId = GetCurrentUserId();
            var createdNote = await _logic.CreateNoteAsync(userId, note);
            return CreatedAtAction(nameof(GetById), new { id = createdNote.NoteId }, createdNote);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    public override async Task<ActionResult<Note>> Update(int id, Note note)
    {
        var userId = GetCurrentUserId();
        var existingNote = await _logic.GetNoteByIdAsync(id, userId);

        if (existingNote == null)
            return NotFound();

        note.NoteId = id;
        note.UserId = userId;
        note.UpdatedAt = DateTime.UtcNow;
        note.IsActive = true;

        var updatedNote = await _logic.UpdateNoteAsync(note);
        return Ok(updatedNote);
    }

    [HttpDelete("{id}")]
    public override async Task<ActionResult> Delete(int id)
    {
        var userId = GetCurrentUserId();
        var result = await _logic.DeleteNoteAsync(id, userId);

        if (!result)
            return NotFound();

        return NoContent();
    }

    // search notes by title
    [HttpGet]
    public async Task<ActionResult<PaginationResponseDto<Note>>> SearchNotes([FromQuery] string title, [FromQuery] PaginationDto pagination)
    {
        var userId = GetCurrentUserId();
        var notes = await _logic.SearchNotesAsync(userId, title, pagination);
        return Ok(notes);
    }

    // This method retrieves the current user's ID from the claims principal.
    // The 'User' property is an instance of ClaimsPrincipal, which represents the current user.
    // It contains claims (key-value pairs) associated with the user, including the NameIdentifier claim,
    // which is typically used to store the user's unique identifier.
    // The userId claim values was set in the JwtTokenGenerator.cs file
    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
            throw new UnauthorizedAccessException("User not authenticated");

        return int.Parse(userIdClaim);
    }
}