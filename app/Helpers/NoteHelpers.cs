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

    public static async Task<Note> Create(User owner, Note note, NoteAppDBContext _context)
    {
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();

        var ownerRole = await RoleHelpers.Create(new Role { 
            Name = "Owner",
            Note = note,
            NoteId = note.Id,
            Owner = true,
            Update = true,
            Delete = true
        }, _context);
        
        await UserNoteHelpers.Create(new UserNote {
            User = owner,
            UserId = owner.Id,
            Note = note,
            NoteId = note.Id,
            Role = ownerRole,
            RoleId = ownerRole.Id
        }, _context);
        
        return note;
    }

    //Check if note exists
    public static bool Exists(int id, NoteAppDBContext _context)
    {
        return (_context.Notes?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}