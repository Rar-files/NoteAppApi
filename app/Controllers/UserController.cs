using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NoteAppAPI.Dtos;
using NoteAppAPI.Models;
using NoteAppAPI.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace NoteAppAPI.Controllers
{
    [Authorize]
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
            return Ok(await _context.Users.ToListAsync());
        }

        // GET: api/User/{id}
        [HttpGet("{userId}")]
        public async Task<ActionResult<User>> GetUser(int userId)
        {   
            try
            {
                var user = await UserHelpers.GetByID(userId, _context);
                return Ok(user);
            }
            catch (Exception)
            {
                return NotFound("User not found");
            }
        }

        // PUT: api/User/{id}
        [HttpPut("{userId}")]
        public async Task<ActionResult<User>> PutUser(int userId, UserDto user)
        {
            User userToUpdate;
            try
            {
                userToUpdate = await UserHelpers.GetByID(userId, _context);
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

        // DELETE: api/User/{id}
        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            User userToDelete;
            try
            {
                userToDelete = await UserHelpers.GetByID(userId, _context);
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
