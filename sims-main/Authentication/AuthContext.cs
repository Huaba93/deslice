using System;
using Microsoft.EntityFrameworkCore; 

namespace SIMS_Authentication
{
    public class AuthContext : DbContext
    {
        public DbSet<User> Users {get; set;}
        public DbSet<Role> Roles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
#if DEBUG
            IConfigurationRoot config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();
            IConfigurationProvider secretProvider = config.Providers.First();
            secretProvider.TryGet("ConnectionString", out var ConnString);
#else
            string ConnString;
            ConnString = Environment.GetEnvironmentVariable("AuthDbConnectionString") ?? "";
            //s Console.WriteLine("ConnectionString: " + ConnString);
#endif
            if (null == ConnString)
            {
                Console.WriteLine("No Connection String declared");
                Environment.Exit(1);
            }
            optionsBuilder.UseSqlServer(ConnString);
        }
 
    }
}

