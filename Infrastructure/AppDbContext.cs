using Microsoft.EntityFrameworkCore;
using Domain.Models;

namespace Infrastructure
{
    public class AppDbContext(DbContextOptions<AppDbContext> options): DbContext(options)
    {
        public DbSet<ValueEntry> Values { get; set; }
        public DbSet<ResultEntry> Results { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResultEntry>().HasKey(x => x.FileName);
            base.OnModelCreating(modelBuilder);
        }
    }
}
