namespace NoteAppAPI.Dtos;

public class RoleDto{
    public int NoteId { get; set; }
    public required string Name { get; set; }
    public bool? Owner {get; set;}
    public bool? Update {get; set;}
    public bool? Delete {get; set;}
}