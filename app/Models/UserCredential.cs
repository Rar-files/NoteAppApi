using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace NoteAppAPI.Models;

public class UserCredential{
    [Key]
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public required User User { get; set; }
    public required string Password { get; set; }
}