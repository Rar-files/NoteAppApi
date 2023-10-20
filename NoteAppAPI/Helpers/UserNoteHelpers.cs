using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Models;

namespace NoteAppAPI.Helpers;

public static class UserNoteHelpers {
    public static async Task<UserNote> GetByID(int id, NoteAppDBContext _context)
    {  
        if (_context.UserNotes == null)
        {
            throw new Exception("Error retrieving user note");
        }

        var userNote = await _context.UserNotes
            .Include(u => u.User)
            .Include(n => n.Note)
            .Include(un => un.Role)
            .FirstAsync(un => un.Id == id);

        if (userNote == null)
        {
            throw new Exception("Error retrieving user note");
        }

        return userNote;
    }

    public static async Task<UserNote> GetByUserIdAndNoteId(int userId, int noteId, NoteAppDBContext _context)
    {  
        if (_context.UserNotes == null)
        {
            throw new Exception("Error retrieving user note");
        }

        var userNote = await _context.UserNotes
            .Include(un => un.User)
            .Include(un => un.Note)
            .Include(un => un.Role)
            .FirstAsync(un => un.NoteId == noteId && un.UserId == userId);

        if (userNote == null)
        {
            throw new Exception("Error retrieving user note");
        }

        return userNote;
    }

    public static async Task<UserNote> Create(UserNote userNote, NoteAppDBContext _context){
        _context.Add(userNote);
        await _context.SaveChangesAsync();
        return userNote;
    }

    //Check if user note exists
    public static bool Exists(int id, NoteAppDBContext _context)
    {
        return (_context.UserNotes?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}