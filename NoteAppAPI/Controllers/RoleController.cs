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
    public class RoleController : ControllerBase
    {
        private readonly NoteAppDBContext _context;

        public RoleController(NoteAppDBContext context, IMapper mapper)
        {
            _context = context;
        }

        // GET: api/Role/Note/{id}
        [HttpGet("Note/{id}", Name = "GetRolesByNote")]
        public async Task<ActionResult<IEnumerable<Role>>> GetRolesByNote(int id)
        {
            if (_context.Roles == null)
            {
                return NotFound("Roles not found");
            }

            return await _context.Roles.Where(r => r.NoteId == id)
            .Include(n => n.Note)
            .ToListAsync();
        }

        // POST: api/Role
        [HttpPost]
        public async Task<ActionResult<Role>> PostRole(RoleDto roleDto){
            Note note;
            try
            {
                note = await NoteHelpers.GetByID(roleDto.NoteId, _context);
            }
            catch (Exception)
            {
                return NotFound("Note not found");
            }

            bool isOwner = true;
            if(roleDto.Owner == null || roleDto.Owner == false)
                isOwner = false;
            
            bool isUpdate = true;
            if(roleDto.Owner == null || roleDto.Owner == false)
                isUpdate = false;
            
            bool isDelete = true;
            if(roleDto.Owner == null || roleDto.Owner == false)
                isDelete = false;

            var role = await RoleHelpers.Create(new Role { 
                Name = roleDto.Name,
                Note = note,
                NoteId = note.Id,
                Owner = isOwner,
                Update = isUpdate,
                Delete = isDelete
            }, _context);

            return CreatedAtRoute("GetRolesByNote", note.Id, role); 
        }
    }
}