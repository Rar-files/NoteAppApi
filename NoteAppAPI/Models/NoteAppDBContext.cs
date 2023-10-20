using Microsoft.EntityFrameworkCore;

namespace NoteAppAPI.Models;

public class NoteAppDBContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public NoteAppDBContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(Configuration.GetConnectionString("PostgresConnection"));
    }

    public DbSet<Note> Notes { get; set; } = null!;

    public DbSet<User> Users { get; set; } = default!;

    public DbSet<UserNote> UserNotes { get; set; } = default!;

    public DbSet<Role> Roles { get; set; } = default!;
}