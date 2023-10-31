using Microsoft.EntityFrameworkCore;
using Server.Models.Entities;

namespace Server.Contexts
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserEntity>(user =>
            {
                user.HasIndex(u => u.Username)
                .IsUnique();
            });

            modelBuilder.Entity<UserTypeEntity>(userType =>
            {
                userType.HasMany(ut => ut.Users)
                .WithOne(u => u.UserType)
                .HasForeignKey(u => u.UserTypeId)
                .OnDelete(DeleteBehavior.NoAction);
            });

            
        }


        public DbSet<UserEntity> Users { get; set; }
        public DbSet<UserTypeEntity> UserTypes { get; set; }
    }
}
