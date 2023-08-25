using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.Reflection.Emit;

namespace GhostUI.UserDB
{
    public class JwtDB: IdentityDbContext<IdentityUser>
    {
        
        public JwtDB(DbContextOptions<JwtDB> options) : base(options)
        {
            

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@Directory.GetCurrentDirectory() + "/../GhostUI/appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<JwtDB>();
            var connectionString = configuration.GetConnectionString("jwtDB");
            optionsBuilder.UseNpgsql(connectionString, opt => opt.MigrationsAssembly("GhostUI.UserDB"));


        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            //This is the functional way on NetCore > 2.2
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();

                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
        }

    }

    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<JwtDB>
    {
        public JwtDB CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(@Directory.GetCurrentDirectory() + "/../GhostUI/appsettings.json")
                .Build();
            var builder = new DbContextOptionsBuilder<JwtDB>();
            var connectionString = configuration.GetConnectionString("jwtDB");
            builder.UseNpgsql(connectionString, opt => opt.MigrationsAssembly("GhostUI.UserDB"));
            return new JwtDB(builder.Options);
        }
    }
}