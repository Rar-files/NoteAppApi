using System.ComponentModel.DataAnnotations;
using AutoMapper;
using NoteAppAPI.Models;

namespace NoteAppAPI.Dtos;

public class NoteDto{
    [Required]
    public string? Title { get; set; }
    [Required]
    public string? Body { get; set; }
}

public class NoteAutoMapperProfile : Profile
{
    public NoteAutoMapperProfile()
    {
        CreateMap<NoteDto, Note>();
    }
}