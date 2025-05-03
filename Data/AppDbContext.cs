using Microsoft.EntityFrameworkCore;
using coffre_fort_api.Models;

namespace coffre_fort_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<PasswordEntry> PasswordEntries { get; set; }
        public DbSet<PasswordShare> PasswordShares { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relation 1-N entre User et PasswordEntry
            modelBuilder.Entity<User>()
                .HasMany(u => u.PasswordEntries)
                .WithOne(p => p.User)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Conversion explicite pour la liste de Tags
            modelBuilder.Entity<PasswordEntry>()
                .Property(p => p.Tags)
                .HasConversion(
                    v => string.Join(',', v),
                    v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
                );

            // Nouvelle logique de partage : utilisateur ? utilisateur
            modelBuilder.Entity<PasswordShare>()
                .HasOne(ps => ps.SourceUser)
                .WithMany()
                .HasForeignKey(ps => ps.SourceUserId)
                .OnDelete(DeleteBehavior.Restrict); // on evite les suppressions en cascade pour les liens

            modelBuilder.Entity<PasswordShare>()
                .HasOne(ps => ps.SharedWithUser)
                .WithMany()
                .HasForeignKey(ps => ps.SharedWithUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
