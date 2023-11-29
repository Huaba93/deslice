using System;
using Microsoft.EntityFrameworkCore; 

namespace SIMS_Backend
{
    public class SIMSContext : DbContext
    {
        public DbSet<Issue> Issues { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<System> Systems { get; set; }
        public DbSet<Systemtype> Systemtypes { get; set; }
        public DbSet<Manufactor> Manufactors { get; set; }
        public DbSet<Logs> Logs { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        //public DbSet<IssueSystem> IssueSystem {get;set;}

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("ConnectionString", out var ConnString);
#else
           string ConnString = Environment.GetEnvironmentVariable("BackendDbConnectionString");
#endif
            optionsBuilder.UseSqlServer(ConnString);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Issue>()
           .HasMany(i => i.AffectedSystems)
           .WithMany();

            //modelBuilder.Entity<IssueSystem>()
            //    .HasKey(issy => new { issy.IssueID, issy.SystemID });

            //modelBuilder.Entity<IssueSystem>()
            //      .HasOne(issy => issy.Issue)
            //      .WithMany(i => i.AffectedSystems)
            //      .HasForeignKey(issy => issy.IssueID);

            //modelBuilder.Entity<IssueSystem>()
            //    .HasOne(issy => issy.System)
            //    .WithMany(s => s.Issues)
            //    .HasForeignKey(issy => issy.SystemID);

        }

    }
}

