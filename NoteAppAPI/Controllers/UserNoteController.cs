using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Models;

namespace NoteAppAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserNoteController : ControllerBase
{
    private readonly NoteAppDBContext _context;

    public UserNoteController(NoteAppDBContext context)
    {
        _context = context;
    }
    


    //-----Endpoints-----

    // GET: api/UserNote
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserNote>>> GetUserNotes()
    {
        if (_context.UserNotes == null)
        {
            return NotFound();
        }
        return await _context.UserNotes.ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserNote>> GetUserNote(int id)
    {
        try
        {
            var userNote = await GetUserNoteById(id);
            return userNote;
        }
        catch (Exception)
        {
            return NotFound();
        }
    }



    //-----Helper functions-----
    
    //Retrive user note by id
    public async Task<UserNote> GetUserNoteById(int id)
    {
        if(_context.UserNotes == null)
        {
            throw new Exception("Error retrieving user note");
        }

        var userNote = await _context.UserNotes.FindAsync(id);

        if(userNote == null)
        {
            throw new Exception("Error retrieving user note");
        }

        return userNote;
    }
}