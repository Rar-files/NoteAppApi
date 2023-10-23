using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteAppAPI.Models;

public class UserNote{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [ForeignKey("NoteId")]
    public int NoteId { get; set; }
    public required Note Note { get; set; }
    [Required]
    [ForeignKey("UserId")]
    public int UserId { get; set; }
    public required User User { get; set; }
    [Required]
    [ForeignKey("RoleId")]
    public int RoleId { get; set; }
    public required Role Role { get; set; }
}