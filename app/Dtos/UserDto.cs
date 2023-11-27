using System.ComponentModel.DataAnnotations;
using AutoMapper;
using NoteAppAPI.Models;

namespace NoteAppAPI.Dtos;

public class UserDto
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

public class UserDtoShorted
{
    [Required]
	[EmailAddress]
    public required string Email { get; set; }
    [Required]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$", ErrorMessage = "Password field is requiered and contains minimum eight characters, at least one letter and one number")]
    public required string Password { get; set; }
}

public class UserAutoMapperProfile : Profile
{
    public UserAutoMapperProfile()
    {
        CreateMap<UserDto, User>();
        CreateMap<UserDto, UserDtoShorted>();
        CreateMap<User, UserDtoShorted>();
    }
}