using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Models;
using NoteAppAPI.Dtos;
using AutoMapper;
using NoteAppAPI.Helpers;
using Microsoft.AspNetCore.Authorization;

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
            if (_context.Notes == null)
            {
                return NotFound("Note not found");
            }

            return await _context.Notes.ToListAsync();
        }

        // GET: api/Note/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            try
            {
                var note = await NoteHelpers.GetByID(id, _context);
                return note;
            }
            catch (Exception)
            {
                
                return NotFound("Note not found");
            }
        }

        // PUT: api/Note/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Note>> PutNote(int id, NoteDto note)
        {
            Note noteToUpdate;
            try
            {
                noteToUpdate = await NoteHelpers.GetByID(id, _context);
            }
            catch (Exception)
            {
                
                return NotFound("Note not found");
            }

            noteToUpdate = _mapper.Map<NoteDto, Note>(note, noteToUpdate);
            noteToUpdate.UpdatedAt = DateTime.UtcNow;

            _context.Entry(noteToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(noteToUpdate);
        }

        // POST: api/Note
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(NoteDto noteDto)
        {
            User owner;
            try
            {
                owner = await UserHelpers.GetByID(noteDto.OwnerId, _context);
            }
            catch (Exception)
            {
                return NotFound("User not found");
            }
            
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
