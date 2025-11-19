using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Kafee.Api.Data;

public class KafeeDbContext(DbContextOptions<KafeeDbContext> options) : DbContext(options)
{
    public DbSet<MenuItem> MenuItems { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}