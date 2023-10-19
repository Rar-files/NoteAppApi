using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NoteAppAPI.Models;

public class Role
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    [ForeignKey("NoteId")]
    public int NoteId { get; set; }
    public required Note Note { get; set; }
    public required string Name { get; set; }
    public bool Owner {get; set;}
    public bool Update {get; set;}
    public bool Delete {get; set;}
}