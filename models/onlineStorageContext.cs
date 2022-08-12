using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;
using OnlineStore.models;

namespace OnlineStore.models
{
    public class OnlineStorageContext : DbContext
    {
        public OnlineStorageContext(DbContextOptions<OnlineStorageContext> options)
            : base(options)
        {
        }

        protected OnlineStorageContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> users { get; set; } = null!;
        public DbSet<Payment> payment { get; set; } = null!;
        public DbSet<Item> items { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>()
            .HasIndex(u => u.userName)
            .IsUnique();
        }
    }
}