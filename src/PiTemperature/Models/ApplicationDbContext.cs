using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.OptionsModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PiTemperature.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        const string adminRole = "admins";

        public DbSet<Sensor> Sensors { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Sensor>().HasKey(m => m.Id);
            base.OnModelCreating(builder);
        }
        public static async Task InitializeDatabaseAsync(IServiceProvider serviceProvider)
        {
            var db = serviceProvider.GetRequiredService<ApplicationDbContext>();

            var sqlDb = db.Database;
            if (sqlDb != null)
            {
                //await sqlDb.EnsureDeletedAsync();
                await sqlDb.EnsureCreatedAsync();
                await CreateAdminUser(serviceProvider);
            }

        }
        private static async Task CreateAdminUser(IServiceProvider serviceProvider)
        {
            var options = serviceProvider.GetRequiredService<IOptions<ApplicationDbContextOptions>>().Value;
            var userMgr = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleMgr = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            if (!await roleMgr.RoleExistsAsync(adminRole))
            {
                await roleMgr.CreateAsync(new IdentityRole(adminRole));
            }

            var user = await userMgr.FindByNameAsync(options.DefaultUsername);
            if (user == null)
            {
                user = new ApplicationUser { UserName = options.DefaultUsername, DisplayName = "Administrator" };
                var userCreationResult = await userMgr.CreateAsync(user, options.DefaultPassword);
                if (userCreationResult.Succeeded)
                {
                    await userMgr.AddToRoleAsync(user, adminRole);
                }
            }
        }
    }
}
