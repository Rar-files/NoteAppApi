using System.ComponentModel.DataAnnotations;
using AutoMapper;
using NoteAppAPI.Models;

namespace NoteAppAPI.Dtos;

public class UserDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required DateTime RegistrationDate { get; set; }
}

public class UserAutoMapperProfile : Profile
{
    public UserAutoMapperProfile()
    {
        CreateMap<User, UserDto>();
    }
}