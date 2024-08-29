using Microsoft.EntityFrameworkCore;
using RSAAPI.Models;

namespace RSAAPI.Database
{
    public class RSAContext : DbContext
    {
        public RSAContext(DbContextOptions<RSAContext> options ) : base( options )
        {

        }

        public DbSet<DbUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RSAContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
