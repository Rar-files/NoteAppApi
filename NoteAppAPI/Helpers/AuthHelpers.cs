using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NoteAppAPI.Dtos;
using NoteAppAPI.Helpers;
using NoteAppAPI.Models;

public class AuthHelpers {
    public static string GenerateToken(User user, IConfiguration _config) 
    {
        var keyString = _config["Auth:Jwt:Key"];
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
            throw new NullReferenceException("Auth:Jwt:Key is not set in the configuration file");
    }

    public static async Task<User> AuthenticateByGoogle(OAuthDto oAuthDto, NoteAppDBContext _context){
        //TODO: OAuth authentication with Google
        if(oAuthDto.AuthCredentials == "TempOAuthGoogleToken")
        {
            User user;
            try
            {
                user = await UserHelpers.GetByID(1, _context);
                return user;
            }
            catch (Exception)
            {
                throw new Exception("User not found");
            }
        }
        throw new Exception("Invalid Google OAuth Token");
    }

    public static async Task<User> AuthenticateByLocalAuth(UserDtoShorted user, NoteAppDBContext _context){
        var currentUser = await _context.Users.FirstOrDefaultAsync(u => 
        u.Email.ToLower() == user.Email.ToLower());

        if(currentUser == null)
        {
            throw new InvalidOperationException();
        }

        if(user.Password != currentUser.Password)
        {
            throw new ArgumentException("Invalid password");
        }

        return currentUser;
    }
}