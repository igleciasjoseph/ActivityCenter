using Microsoft.EntityFrameworkCore;

namespace ActivityCenter.Models
{
    public class ACContext : DbContext
    {
        public ACContext(DbContextOptions options) : base(options) { }
        public DbSet<User> UserList { get; set; }
        public DbSet<Occasion> ActList { get; set; }
        public DbSet<Join> Joinee { get; set; }
    }
}