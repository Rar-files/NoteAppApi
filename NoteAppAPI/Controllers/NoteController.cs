using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Models;
using NoteAppAPI.Dtos;
using AutoMapper;

namespace NoteAppAPI.Controllers
{
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



        // -----Endpoints-----

        // GET: api/Note
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetNotes()
        {
            if (_context.Notes == null)
            {
                return NotFound();
            }

            return await _context.Notes.ToListAsync();
        }

        // GET: api/Note/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<Note>> GetNote(int id)
        {
            try
            {
                var note = await GetNoteByID(id);
                return note;
            }
            catch (Exception)
            {
                
                return NotFound();
            }
        }

        // PUT: api/Note/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<Note>> PutNote(int id, NoteDto note)
        {
            Note noteToUpdate;
            try
            {
                noteToUpdate = await GetNoteByID(id);
            }
            catch (Exception)
            {
                
                return NotFound();
            }

            noteToUpdate = _mapper.Map<NoteDto, Note>(note, noteToUpdate);
            noteToUpdate.UpdatedAt = DateTime.UtcNow;

            _context.Entry(noteToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(noteToUpdate);
        }

        // POST: api/Note
        [HttpPost]
        public async Task<ActionResult<Note>> PostNote(NoteDto note)
        {
            var noteToCreate = _mapper.Map<Note>(note);
            noteToCreate.CreatedAt = DateTime.UtcNow;
            noteToCreate.UpdatedAt = DateTime.UtcNow;
            
            _context.Notes.Add(noteToCreate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNote), new { id = noteToCreate.Id }, noteToCreate);
        }

        // DELETE: api/Note/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNote(int id)
        {
            Note noteToDelete;
            try
            {
                noteToDelete = await GetNoteByID(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            _context.Notes.Remove(noteToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }



        //-----Helper functions-----

        //Retrieve note by ID
        private async Task<Note> GetNoteByID(int id)
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
        private bool NoteExists(int id)
        {
            return (_context.Notes?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
