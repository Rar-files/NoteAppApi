using System.ComponentModel.DataAnnotations;

namespace NoteAppAPI.Dtos;

public class UserNoteDto{
    [Required]
    public int NoteId { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int RoleId { get; set; }
}