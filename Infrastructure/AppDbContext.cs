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
            modelBuilder.Entity<ValueEntry>(entity =>
            {
                entity.ToTable("Values");
                entity.HasKey("Id");
                entity.HasIndex(e => new { e.FileName, e.Date });
                entity.Property(e => e.FileName).IsRequired();
                entity.Property(e => e.Date).IsRequired();
                entity.Property(e => e.Value).IsRequired();
                entity.Property(e => e.ExecutionTime).IsRequired();
            });
            modelBuilder.Entity<ResultEntry>(entity =>
            {
                entity.ToTable("Results");
                entity.HasKey(x => x.FileName);
                entity.Property(e => e.FileName).IsRequired();
            });

                
            base.OnModelCreating(modelBuilder);
        }
    }
}
