using NoteAppAPI.Models;

namespace NoteAppAPI.Helpers;

public static class UserHelpers {
    public static async Task<User> GetByID(int id, NoteAppDBContext _context)
    {  
        if (_context.Users == null)
        {
            throw new Exception("Error retrieving user");
        }

        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            throw new Exception("Error retrieving user");
        }

        return user;
    }

    //Check if user exists
    public static bool Exists(int id, NoteAppDBContext _context)
    {
        return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}