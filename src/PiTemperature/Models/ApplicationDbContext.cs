using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.Extensions.PlatformAbstractions;
using System.IO;

namespace PiTemperature.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        const string adminRole = "admins";

        public DbSet<ApplicationDbMigrationHistory> DbMigrationHistory { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationDbMigrationHistory>().HasKey(m => m.Id);
            builder.Entity<ApplicationDbMigrationHistory>().HasAlternateKey(e => e.MigrationId);
            builder.Entity<Sensor>().HasKey(m => m.Id);
            base.OnModelCreating(builder);
        }
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider, ILoggerFactory loggerFactory)
        {
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var logger = loggerFactory.CreateLogger("Initialize Database");
            var sqlDb = db.Database;
            if (sqlDb != null)
            {
                //await sqlDb.EnsureDeletedAsync();
                var b = await sqlDb.EnsureCreatedAsync();
                if (b)
                {
                    logger.LogInformation("Database created!");
                    await Migrate(db, logger, true);
                }
                else
                {
                    await Migrate(db, logger);
                }

                logger.LogInformation("Creating default Roles and Users if necessary!");
                await CreateAdminUser(serviceProvider, logger);
            }

        }
        private static async Task CreateAdminUser(IServiceProvider serviceProvider, ILogger logger)
        {
            var options = serviceProvider.GetRequiredService<IOptions<ApplicationDbContextOptions>>().Value;
            var userMgr = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleMgr.RoleExistsAsync(adminRole))
            {
                logger.LogInformation("Adding role {role}.", adminRole);
                await roleMgr.CreateAsync(new IdentityRole(adminRole));
            }

            var user = await userMgr.FindByNameAsync(options.DefaultUsername);
            if (user == null)
            {
                logger.LogInformation("Adding user {user}.", options.DefaultUsername);
                user = new ApplicationUser { UserName = options.DefaultUsername, DisplayName = "Administrator" };
                var userCreationResult = await userMgr.CreateAsync(user, options.DefaultPassword);
                if (userCreationResult.Succeeded)
                {
                    logger.LogInformation("Adding user {user} to role {role}.", options.DefaultUsername, adminRole);
                    await userMgr.AddToRoleAsync(user, adminRole);
                }
            }
        }

        class DBMigrations
        {
            public int MigrationMax { get; set; }
            public string[] Migrations { get; set; }
            public string[] Notes { get; set; }
        }
        private static async Task Migrate(ApplicationDbContext db, ILogger logger, bool dbCreated = false)
        {
            DBMigrations dbMigrations = GetMigrations();
            if (dbCreated)
            {
                logger.LogInformation("Adding default migration.");
                db.DbMigrationHistory.Add(new ApplicationDbMigrationHistory() { MigrationId = dbMigrations.MigrationMax, Note = "Initial DB" });
                await db.SaveChangesAsync();
            }
            else
            {
                logger.LogInformation("Adding migrations if necessary!");
                int actualMigration = db.DbMigrationHistory.Max(c => c.MigrationId);
                for (int i = actualMigration; i < dbMigrations.MigrationMax; i++)
                {
                    string sql = dbMigrations.Migrations[i];
                    string note = dbMigrations.Notes[i];
                    await db.Database.BeginTransactionAsync();
                    try
                    {

                        logger.LogInformation("Adding migration {0}.",note);
                        await db.Database.ExecuteSqlCommandAsync(sql);
                        db.DbMigrationHistory.Add(new ApplicationDbMigrationHistory() { MigrationId = i + 1, Note = note });
                        db.SaveChanges();
                        db.Database.CommitTransaction();
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Database Error", ex);
                        db.Database.RollbackTransaction();
                    }
                }
            }
        }

        private static DBMigrations GetMigrations()
        {
            var path = PlatformServices.Default.Application.ApplicationBasePath;
            var fileName = Path.Combine(path, "migrations.json");
            if (File.Exists(fileName))
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
                {
                    using (StreamReader sr = new StreamReader(fs))
                    {
                        string json = sr.ReadToEnd();
                        var dbMigrations = JsonConvert.DeserializeObject<DBMigrations>(json);
                        if (dbMigrations != null)
                            return dbMigrations;
                        else
                            return new DBMigrations();
                    }
                }
            }
            else
                return new DBMigrations();
        }
    }
}
