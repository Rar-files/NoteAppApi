using AutoMapper;
using NoteAppAPI.Models;

namespace NoteAppAPI.Dtos;

public class UserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public class UserDtoShorted
{
    public required string Email { get; set; }
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