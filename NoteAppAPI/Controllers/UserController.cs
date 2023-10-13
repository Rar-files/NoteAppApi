using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Dtos;
using NoteAppAPI.Models;
using NoteAppAPI.Helpers;

namespace NoteAppAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly NoteAppDBContext _context;
        private readonly IMapper _mapper;

        public UserController(NoteAppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            if (_context.Users == null)
            {
                return NotFound("Users not found");
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {   
            try
            {
                var user = await UserHelpers.GetByID(id, _context);
                return user;
            }
            catch (Exception)
            {
                return NotFound("User not found");
            }
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDto user)
        {
            User userToUpdate;
            try
            {
                userToUpdate = await UserHelpers.GetByID(id, _context);
            }
            catch (Exception)
            {
                return NotFound("User not found");
            }

            userToUpdate = _mapper.Map<UserDto, User>(user, userToUpdate);

            _context.Entry(userToUpdate).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(userToUpdate);
        }

        // POST: api/User
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(UserDto user)
        {
            var userToCreate = _mapper.Map<User>(user);
            userToCreate.RegistrationDate = DateTime.UtcNow;

            _context.Users.Add(userToCreate);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUser), new { id = userToCreate.Id }, userToCreate);
        }

        // DELETE: api/User/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            User userToDelete;
            try
            {
                userToDelete = await UserHelpers.GetByID(id, _context);
            }
            catch (Exception)
            {
                return NotFound("User not found");
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
