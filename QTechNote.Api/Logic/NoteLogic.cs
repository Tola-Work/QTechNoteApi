using Microsoft.EntityFrameworkCore;
using QTechNote.Data.Models;
using QTechNote.Models.DTOs;

namespace QTechNote.Api.Logic;

public class NoteLogic : BaseLogic<Note>
{
    public NoteLogic(QTechNoteContext context) : base(context) { }

    public async Task<PaginationResponseDto<Note>> GetUserNotesAsync(int userId, PaginationDto pagination)
    {
        var totalCount = await _dbSet.CountAsync(n => n.UserId == userId && n.IsActive);
        var notes = await _dbSet
            .Where(n => n.UserId == userId && n.IsActive)
            .OrderByDescending(n => n.UpdatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginationResponseDto<Note>
        {
            Data = notes,
            TotalPages = (int)Math.Ceiling((double)totalCount / pagination.PageSize),
            CurrentPage = pagination.Page
        };
    }

    public async Task<Note?> GetNoteByIdAsync(int noteId, int userId)
    {
        return await _dbSet
            .FirstOrDefaultAsync(n =>
                n.NoteId == noteId &&
                n.UserId == userId &&
                n.IsActive);
    }

    public async Task<Note> CreateNoteAsync(int userId, Note note)
    {
        note.UserId = userId;
        note.CreatedAt = DateTime.UtcNow;
        note.UpdatedAt = DateTime.UtcNow;
        note.IsActive = true;

        await _dbSet.AddAsync(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task<Note> UpdateNoteAsync(Note note)
    {
        var existingNote = await _dbSet.FindAsync(note.NoteId);
        if (existingNote == null)
            throw new KeyNotFoundException($"Note with ID {note.NoteId} not found");

        existingNote.UpdatedAt = DateTime.UtcNow;
        _context.Entry(existingNote).CurrentValues.SetValues(note);
        await _context.SaveChangesAsync();
        return existingNote;
    }

    public async Task<bool> DeleteNoteAsync(int noteId, int userId)
    {
        var note = await GetNoteByIdAsync(noteId, userId);
        if (note == null)
            return false;

        // Soft delete
        note.IsActive = false;
        await _context.SaveChangesAsync();
        return true;
    }

    // search notes by title
    public async Task<PaginationResponseDto<Note>> SearchNotesAsync(int userId, string title, PaginationDto pagination)
    {
        var totalCount = await _dbSet
            .Where(n => n.Title.Contains(title) && n.IsActive)
            .CountAsync();

        var notes = await _dbSet
            .Where(n => n.Title.Contains(title) && n.IsActive && n.UserId == userId)
            .OrderByDescending(n => n.UpdatedAt)
            .Skip((pagination.Page - 1) * pagination.PageSize)
            .Take(pagination.PageSize)
            .ToListAsync();

        return new PaginationResponseDto<Note>
        {
            Data = notes,
            TotalPages = (int)Math.Ceiling((double)totalCount / pagination.PageSize),
            CurrentPage = pagination.Page
        };
    }
}
