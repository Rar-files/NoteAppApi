using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

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
}