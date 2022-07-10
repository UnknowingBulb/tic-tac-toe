using Microsoft.EntityFrameworkCore;
using XOX.Models;

namespace XOX.Database
{
    public class SessionContext : DbContext
    {
        public DbSet<SessionModel> Sessions { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<UserSessionsModel> UserSessions { get; set; }

        public SessionContext(DbContextOptions<SessionContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SessionModel>().ToTable("SessionModel").HasKey(m => new { m.Id}); ;
            modelBuilder.Entity<UserModel>().ToTable("UserModel").HasKey(m => new { m.Id }); ;
            modelBuilder.Entity<UserSessionsModel>().ToTable("UserSessionsModel").HasKey(m => new { m.UserModelId, m.SessionModelId }); 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Sessionss.db");
        }
    }
}