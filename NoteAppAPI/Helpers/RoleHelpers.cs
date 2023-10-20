using NoteAppAPI.Models;

namespace NoteAppAPI.Helpers;

public static class RoleHelpers {
    public static async Task<Role> GetByID(int id, NoteAppDBContext _context)
    {  
        if (_context.Roles == null)
        {
            throw new Exception("Error retrieving role");
        }

        var role = await _context.Roles.FindAsync(id);

        if (role == null)
        {
            throw new Exception("Error retrieving role");
        }

        return role;
    }

    public static async Task<Role> Create(Role role, NoteAppDBContext _context){
        _context.Add(role);
        await _context.SaveChangesAsync();
        return role;
    }

    //Check if user exists
    public static bool Exists(int id, NoteAppDBContext _context)
    {
        return (_context.Roles?.Any(e => e.Id == id)).GetValueOrDefault();
    }
}