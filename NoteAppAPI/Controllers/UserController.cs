using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Dtos;
using NoteAppAPI.Models;

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



        // -----Endpoints-----

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {   
            try
            {
                var user = await GetUserByID(id);
                return user;
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        // PUT: api/User/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, UserDto user)
        {
            User userToUpdate;
            try
            {
                userToUpdate = await GetUserByID(id);
            }
            catch (Exception)
            {
                return NotFound();
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
                userToDelete = await GetUserByID(id);
            }
            catch (Exception)
            {
                return NotFound();
            }

            _context.Users.Remove(userToDelete);
            await _context.SaveChangesAsync();

            return Ok();
        }



        //-----Helper functions-----

        //Retrieve user by id
        public async Task<User> GetUserByID(int id)
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
        private bool UserExists(int id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
