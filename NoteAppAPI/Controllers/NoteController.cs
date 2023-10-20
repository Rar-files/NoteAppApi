using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Models;
using NoteAppAPI.Dtos;
using AutoMapper;
using NoteAppAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace NoteAppAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NoteController : ControllerBase
    {
        private readonly NoteAppDBContext _context;
        private readonly IMapper _mapper;

        public NoteController(NoteAppDBContext context,IMapper
        mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Note
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if(userId is not null)
            {
                return await (
                    from un in _context.UserNotes
                    where un.UserId == int.Parse(userId)
                    join notes in _context.Notes on un.NoteId equals notes.Id
                    select notes
                ).ToListAsync();
            }
            else
                return BadRequest("Bad JWT claimes");
        }

        // GET: api/Note/{noteId}
        [HttpGet("{noteId}")]
        public async Task<ActionResult<Note>> GetNote(int noteId)
        {
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if(userId is not null)
            {
                try{
                    return (await UserNoteHelpers.GetByUserIdAndNoteId(int.Parse(userId),noteId,_context)).Note;
                }
                catch (Exception)
                {
                    return NotFound("No note assigned to the user was found with that ID");
                }
            }
            else
                return BadRequest("Bad JWT claimes");
        }

        // PUT: api/Note/{id}
        [HttpPut("{noteId}")]
        public async Task<ActionResult<Note>> PutNote(int noteId, NoteDto note)
        {
            Note noteToUpdate;
            Role userRole;
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if(userId is not null)
            {
                try{
                    var un = await UserNoteHelpers.GetByUserIdAndNoteId(int.Parse(userId),noteId,_context);
                    noteToUpdate = un.Note;
                    userRole = un.Role;
                }
                catch (Exception)
                {
                    return NotFound("No note assigned to the user was found with that ID");
                }
            }
            else
                return BadRequest("Bad JWT claimes");
            
            if(userRole.Owner || userRole.Update)
            {
                noteToUpdate = _mapper.Map<NoteDto, Note>(note, noteToUpdate);
                noteToUpdate.UpdatedAt = DateTime.UtcNow;

                _context.Entry(noteToUpdate).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(noteToUpdate);
            }
            else
                return Unauthorized("Insufficient permissions to edit the note with the provided ID");
        }

        // POST: api/Note
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(NoteDto noteDto)
        {
            User owner;
            var userId = User.FindFirst(ClaimTypes.Sid)?.Value;
            if(userId is not null)
            {
                try
                {
                    owner = await UserHelpers.GetByID(int.Parse(userId), _context);
                }
                catch (Exception)
                {
                    return NotFound("User not found");
                }
            }
            else
                return BadRequest("Bad JWT claimes");
            
            var noteToCreate = _mapper.Map<Note>(noteDto);
            var actualTime = DateTime.UtcNow;
            noteToCreate.CreatedAt = actualTime;
            noteToCreate.UpdatedAt = actualTime;

            var noteCreated = await NoteHelpers.Create(owner, noteToCreate, _context);

            return CreatedAtAction(nameof(GetNote), new { id = noteToCreate.Id }, noteToCreate);
        }

        // DELETE: api/Note/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            Note noteToDelete;
            try
            {
                noteToDelete = await NoteHelpers.GetByID(id, _context);
            }
            catch (Exception)
            {
                return NotFound("Note not found");
            }

            _context.Notes.Remove(noteToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
