using System.ComponentModel.DataAnnotations;
using AutoMapper;
using NoteAppAPI.Models;

namespace NoteAppAPI.Dtos;

public class UserCredentialDto
{
    [Required]
	[EmailAddress]
    public required string Email { get; set; }
    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password field is requiered and contains minimum eight characters, at least one letter and one number")]
    public required string Password { get; set; }
    [Required]
    public required string FirstName { get; set; }
    [Required]
    public required string LastName { get; set; }
}

public class UserCredentialDtoShorted
{
    [Required]
	[EmailAddress]
    public required string Email { get; set; }
    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password field is requiered and contains minimum eight characters, at least one letter and one number")]
    public required string Password { get; set; }
}

public class AuthUserAutoMapperProfile : Profile
{
    public AuthUserAutoMapperProfile()
    {
        CreateMap<UserCredentialDto, User>();
        CreateMap<UserCredentialDto, UserCredentialDtoShorted>();
        CreateMap<User, UserCredentialDtoShorted>();
    }
}