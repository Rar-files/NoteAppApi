using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NoteAppAPI.Dtos;
using NoteAppAPI.Helpers;
using NoteAppAPI.Models;

public class AuthHelpers {
    public static string GenerateToken(User user, IConfiguration _config) 
    {
        var keyString = _config["Secrets:JWTKey"];
        if(keyString is not null)
        {
        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, user.Email),
            new Claim(ClaimTypes.Sid, user.Id.ToString())
        };
        var token = new JwtSecurityToken(
            _config["Auth:Jwt:Issuer"], 
            _config["Auth:Jwt:Audience"], 
            claims, 
            expires: DateTime.Now.AddMinutes(15), 
            signingCredentials: creds);
        
        return new JwtSecurityTokenHandler().WriteToken(token);
        }
        else
            throw new NullReferenceException("Secrets:JWTKey is not set in the configuration file");
    }

    public static async Task<User> AuthenticateByLocalAuth(UserCredentialDtoShorted userCredential, NoteAppDBContext _context){
        var currentUserCreditial = await _context.UserCredentials
        .Include(uc => uc.User)
        .FirstAsync(u => u.User.Email.ToLower() == userCredential.Email.ToLower());

        if(currentUserCreditial == null)
        {
            throw new InvalidOperationException();
        }

        if(userCredential.Password != currentUserCreditial.Password)
        {
            throw new ArgumentException("Invalid password");
        }

        return currentUserCreditial.User;
    }

    
    public static async Task<UserCredential> GetCreditialByID(int id, NoteAppDBContext _context)
    {
        if (_context.UserCredentials == null)
        {
            throw new Exception("Error retrieving user credential");
        }

        var userCredential = await _context.UserCredentials
            .Include(uc => uc.User)
            .FirstAsync(uc => uc.UserId == id);

        if (userCredential == null)
        {
            throw new Exception("Error retrieving user credential");
        }

        return userCredential;
    }

    public static async Task<UserCredential> CreateUser(UserCredentialDto userCredentialDto, NoteAppDBContext _context, IMapper _mapper)
    {
        var userToCreate = _mapper.Map<User>(userCredentialDto);
        userToCreate.RegistrationDate = DateTime.UtcNow;
        await UserHelpers.Create(userToCreate, _context);
        await _context.SaveChangesAsync();

        var userCreditialToCreate = new UserCredential{
            UserId = userToCreate.Id,
            User = userToCreate,
            Password = userCredentialDto.Password
        };

        _context.UserCredentials.Add(userCreditialToCreate);
        await _context.SaveChangesAsync();
        
        return userCreditialToCreate;
    }
}