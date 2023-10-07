using NoteAppAPI.Models;

namespace NoteAppAPI.Helpers;

public static class NoteHelpers {
    public static async Task<Note> GetByID(int id, NoteAppDBContext _context)
    {  
        if (_context.Notes == null)
        {
            throw new Exception("Error retrieving note");
        }

        var note = await _context.Notes.FindAsync(id);

        if (note == null)
        {
            throw new Exception("Error retrieving note");
        }

        return note;
    }

    //Check if note exists
    public static bool Exists(int id, NoteAppDBContext _context)
    {
        return (_context.Notes?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}