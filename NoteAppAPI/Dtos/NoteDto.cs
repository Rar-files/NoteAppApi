using AutoMapper;
using NoteAppAPI.Models;

namespace NoteAppAPI.Dtos;

public class NoteDto{
    public int OwnerId { get; set; }
    public string? Title { get; set; }
    public string? Body { get; set; }
}

public class NoteAutoMapperProfile : Profile
{
    public NoteAutoMapperProfile()
    {
        CreateMap<NoteDto, Note>();
    }
}