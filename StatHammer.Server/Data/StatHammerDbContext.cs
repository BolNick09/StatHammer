using Microsoft.EntityFrameworkCore;
using StatHammer.Server.Models.Entities;

namespace StatHammer.Server.Data
{
    public class StatHammerDbContext : DbContext
    {
        public StatHammerDbContext(DbContextOptions<StatHammerDbContext> options)
            : base(options)
        {
        }

        public DbSet<Unit> Units { get; set; }
    }
}
