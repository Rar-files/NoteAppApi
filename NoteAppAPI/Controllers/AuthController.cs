using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NoteAppAPI.Dtos;
using NoteAppAPI.Helpers;
using NoteAppAPI.Models;

namespace NoteAppAPI.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly NoteAppDBContext _context;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;

    public AuthController(NoteAppDBContext context, IConfiguration config, IMapper mapper)
    {
        _context = context;
        _config = config;
        _mapper = mapper;
    }
    
    public interface ILoginResponse
    {
        string Token { get; set; }
    }

    [HttpPost("Google")]
    public async Task<ActionResult<ILoginResponse>> LoginGoogle(OAuthDto oAuthDto)
    {
        if(oAuthDto == null)
            return BadRequest();

        try
        {
            var authedUser = await AuthHelpers.AuthenticateByGoogle(oAuthDto, _context);
            return Ok(new { Token = AuthHelpers.GenerateToken(authedUser,_config)});
        }
        catch (Exception e)
        {
            return Unauthorized(e.Message);
        }
    }

    [HttpPost("Local/Login")]
    public async Task<ActionResult<ILoginResponse>> LoginLocalLogin(UserDtoShorted userCred)
    {
        if(userCred == null)
            return BadRequest();

        try
        {
            var authedUser = await AuthHelpers.AuthenticateByLocalAuth(userCred, _context);
            return Ok(new { Token = AuthHelpers.GenerateToken(authedUser,_config)});
        }
        catch (InvalidOperationException)
        {
            return BadRequest($"User not found, please sign up: {Request.Scheme}://{Request.Host}/api/Local/Signup");
        }
        catch (ArgumentException e)
        {
            return Unauthorized(e.Message);
        }
    }

    public interface ILoginLocalSignupResponse
    {
        string Token { get; set; }
        User User { get; set; }
    }

    [HttpPost("Local/Signup")]
    public async Task<ActionResult<ILoginLocalSignupResponse>> LoginLocalSignup(UserDto userDto)
    {
        if(userDto == null)
            return BadRequest();
        if(userDto.FirstName == null)
            return BadRequest("First name is missing");
        if(userDto.LastName == null)
            return BadRequest("Last name is missing");
        if(UserHelpers.ExistsByEmail(userDto.Email, _context))
            return BadRequest("User with this email already exists");

        var userToCreate = _mapper.Map<User>(userDto);
            userToCreate.RegistrationDate = DateTime.UtcNow;

        _context.Users.Add(userToCreate);
        await _context.SaveChangesAsync();

        var authedUser = await AuthHelpers.AuthenticateByLocalAuth(_mapper.Map<UserDtoShorted>(userToCreate), _context);
        var token = AuthHelpers.GenerateToken(authedUser,_config);

        return CreatedAtAction(actionName: nameof(UserController.GetUser), controllerName: nameof(UserController)[0..^10], routeValues: new { id = userToCreate.Id }, value: new{ Token = token, User = userToCreate });
    }
}
