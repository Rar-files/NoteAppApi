using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Dtos;
using NoteAppAPI.Helpers;
using NoteAppAPI.Models;

namespace NoteAppAPI.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class UserNoteController : ControllerBase
{
    private readonly NoteAppDBContext _context;

    public UserNoteController(NoteAppDBContext context)
    {
        _context = context;
    }

    // GET: api/UserNote
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserNote>>> GetUserNotes()
    {
        if (_context.UserNotes == null)
        {
            return NotFound();
        }
        return await _context.UserNotes
            .Include(n => n.User)
            .Include(n => n.Note)
            .Include(n => n.Role)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserNote>> GetUserNote(int id)
    {
        try
        {
            var userNote = await UserNoteHelpers.GetByID(id, _context);
            return userNote;
        }
        catch (Exception)
        {
            return NotFound();
        }
    }

    // PUT: api/UserNote/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> PutUserNote(int id, UserNoteDto userNoteDto)
    {
        User user;
        try
        {
            user = await UserHelpers.GetByID(userNoteDto.UserId, _context);
        }
        catch (Exception)
        {
            return NotFound("User not found");
        }

        Note note;
        try
        {
            note = await NoteHelpers.GetByID(userNoteDto.NoteId, _context);
        }
        catch (Exception)
        {
            return NotFound("Note not found");
        }

        UserNote userNoteToUpdate;
        try
        {
            userNoteToUpdate = await UserNoteHelpers.GetByID(id, _context);
        }
        catch (Exception)
        {
            return NotFound("User note not found");
        }

        userNoteToUpdate.Note = note;
        userNoteToUpdate.User = user;

        _context.Entry(userNoteToUpdate).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(userNoteToUpdate);
    }

    // POST: api/UserNote
    [HttpPost]
    public async Task<ActionResult<UserNote>> PostUserNote(UserNoteDto userNoteDto)
    {
        User user;
        try
        {
            user = await UserHelpers.GetByID(userNoteDto.UserId, _context);
        }
        catch (Exception)
        {
            return NotFound("User not found");
        }

        Note note;
        try
        {
            note = await NoteHelpers.GetByID(userNoteDto.NoteId, _context);
        }
        catch (Exception)
        {
            return NotFound("Note not found");
        }

        Role role;
        try
        {
            role = await RoleHelpers.GetByID(userNoteDto.RoleId, _context);
        }
        catch (Exception)
        {
            return NotFound("Role not found");
        }

        if(role.NoteId != note.Id)
        {
            return BadRequest("Role couldn't assign to this note");
        }

        var userNote = await UserNoteHelpers.Create(new UserNote(){ Note = note, User = user , Role = role}, _context);

        return CreatedAtAction("GetUserNote", new { id = userNote.Id }, userNote);

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUserNote(int id)
    {
        UserNote userNoteToUpdate;
        try
        {
            userNoteToUpdate = await UserNoteHelpers.GetByID(id, _context);
        }
        catch (Exception)
        {
            return NotFound("User note not found");
        }

        _context.UserNotes.Remove(userNoteToUpdate);
        await _context.SaveChangesAsync();
        
        return Ok();
    }
}