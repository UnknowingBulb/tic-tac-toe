using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
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
            modelBuilder.Entity<SessionModel>().ToTable("SessionModel");
            modelBuilder.Entity<UserModel>().ToTable("UserModel");
            modelBuilder.Entity<UserSessionsModel>().ToTable("UserSessionsModel").HasKey(m => new { m.UserModelId, m.SessionModelId }); 
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                   .SetBasePath(Directory.GetCurrentDirectory())
                   .AddJsonFile("appsettings.json")
                   .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseSqlite(connectionString);
            }
        }
        /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=Sessions.db");
        }*/
    }
}