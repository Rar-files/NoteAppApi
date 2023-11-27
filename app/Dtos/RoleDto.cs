using System.ComponentModel.DataAnnotations;

namespace NoteAppAPI.Dtos;

public class RoleDto{
    [Required]
    public int NoteId { get; set; }
    [Required]
    public required string Name { get; set; }
    public bool? Owner {get; set;}
    public bool? Update {get; set;}
    public bool? Delete {get; set;}
}