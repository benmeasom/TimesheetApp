using DataObjects.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DataObjects.Context
{
    //https://stackoverflow.com/questions/48712868/avoid-discriminator-with-aspnetusers-aspnetroles-aspnetuserroles
    public class TimesheetDBContext : IdentityDbContext<TimesheetUser, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public TimesheetDBContext(DbContextOptions<TimesheetDBContext> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);


            builder.Entity<Timesheet>(entity =>
            {
                entity.HasMany(q => q.TimesheetItems)
                .WithOne(cq => cq.Timesheet)
                .HasForeignKey(cq => cq.TimesheetId);
            });

            builder.Entity<TimesheetItems>(entity =>
            {
                entity.HasOne(q => q.Timesheet)
                .WithMany(cq => cq.TimesheetItems)
                .HasForeignKey(cq => cq.TimesheetId);
            });

            builder.Entity<TimesheetItems>(entity =>
            {
                entity.HasOne(q => q.Activity)
                .WithMany(cq => cq.TimesheetItems)
                .HasForeignKey(cq => cq.ActivityId);

                entity.HasOne(q => q.ActivityType)
               .WithMany(cq => cq.TimesheetItems)
               .HasForeignKey(cq => cq.ActivityTypeId);
            });
        }
        public DbSet<ActivityType> ActivityTypes { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<Timesheet> Timesheet { get; set; }
        public DbSet<TimesheetItems> TimesheetItems { get; set; }
        public DbSet<TimesheetUser> TimesheetUsers { get; set; }
    }
}
