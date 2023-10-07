using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Dtos;
using NoteAppAPI.Helpers;
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

        var userNote = new UserNote(){ Note = note, User = user };
        _context.Add(userNote);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUserNote", new { id = userNote.Id }, userNote);

    }
}