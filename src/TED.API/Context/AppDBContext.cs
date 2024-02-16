using Microsoft.EntityFrameworkCore;
using TED.API.Entities;

namespace TED.API.Context
{
    public class AppDBContext : DbContext
    {
        public AppDBContext(DbContextOptions<AppDBContext> options)
            : base(options) {}

        public DbSet<Ted> Ted { get; set; }
        public DbSet<LimiteTed> LimiteTed { get; set; }
    }
}