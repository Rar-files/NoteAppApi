using AutoMapper;
using NoteAppAPI.Models;

namespace NoteAppAPI.Dtos;

public class UserDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

public class UserAutoMapperProfile : Profile
{
    public UserAutoMapperProfile()
    {
        CreateMap<UserDto, User>();
    }
}