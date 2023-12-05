using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

    [HttpPost("Local/Login")]
    public async Task<ActionResult<ILoginResponse>> LoginLocalLogin(UserCredentialDtoShorted userCred)
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
            return NotFound($"User not found, please sign up: {Request.Scheme}://{Request.Host}/api/Local/Signup");
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
    public async Task<ActionResult<ILoginLocalSignupResponse>> LoginLocalSignup(UserCredentialDto userCredentialDto)
    {
        if(userCredentialDto == null)
            return BadRequest();
        if(UserHelpers.ExistsByEmail(userCredentialDto.Email, _context))
            return BadRequest("User with this email already exists");

        var userCredential = await AuthHelpers.CreateUser(userCredentialDto, _context, _mapper);

        var token = AuthHelpers.GenerateToken(userCredential.User,_config);

        return CreatedAtAction(actionName: nameof(UserController.GetUser), controllerName: nameof(UserController)[0..^10], routeValues: new { id = userCredential.UserId }, value: new{ Token = token, User = userCredential.User });
    }

    [Authorize]
    [HttpPut("Local/ChangePassword")]
    public async Task<IActionResult> PutAuthUser(UserCredentialDtoShorted userCredential)
    {
        var authedUserId = User.FindFirst(ClaimTypes.Sid)?.Value;

        if(authedUserId == null)
            return StatusCode(418);
            
        UserCredential userCreditialToUpdate;
        try
        {
            userCreditialToUpdate = await AuthHelpers.GetCreditialByID(int.Parse(authedUserId), _context);
        }
        catch (Exception)
        {
            return NotFound("User not found");
        }

        userCreditialToUpdate.Password = userCredential.Password;

        _context.Entry(userCreditialToUpdate).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok();
    }
}
