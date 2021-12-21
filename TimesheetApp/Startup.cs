
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using DataObjects.Context;
using DataObjects.Models;
using BLL;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;

namespace TimesheetApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<TimesheetDBContext>(options =>
             options.UseSqlServer(
                 Configuration.GetConnectionString("TimesheetDBContextConnection")));
            services.AddDefaultIdentity<TimesheetUser>().AddRoles<Role>()
                .AddEntityFrameworkStores<TimesheetDBContext>().AddDefaultUI();

            //services.AddDbContext<TimesheetDBContext>(options =>
            //      options.UseSqlServer(Configuration.GetConnectionString("TimesheetDBContextConnection")));


            services.AddControllersWithViews().AddRazorRuntimeCompilation();
            services.AddRazorPages().AddRazorRuntimeCompilation();
            services.AddSession();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings.
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 0;
                options.Password.RequiredUniqueChars = 0;

                // Lockout settings.
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                //options.Lockout.MaxFailedAccessAttempts = 5;
                //options.Lockout.AllowedForNewUsers = true;

                // User settings.
                options.User.AllowedUserNameCharacters =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                //options.User.RequireUniqueEmail = false;
            });

            services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = false;
                options.ExpireTimeSpan = TimeSpan.FromDays(1);

                options.LoginPath = "/login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;
            });

            services.AddBLLDependency();
            services.AddDALDependency();

            var enGb = new CultureInfo("en-GB");
            enGb.DateTimeFormat.ShortDatePattern = "dd/MM/yyyy";
            enGb.DateTimeFormat.DateSeparator = "/";
            CultureInfo[] supportedCultures = new[]
            {
                enGb
            };

            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new RequestCulture("en-GB");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.Use(async (context, next) =>
            {
                await next();
                if (context.Response.StatusCode == 404)
                {
                    context.Request.Path = "/not-found";
                    await next();
                }
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            //https://www.codemag.com/Article/2009081/A-Deep-Dive-into-ASP.NET-Core-Localization
            var options = app.ApplicationServices.GetService<IOptions<RequestLocalizationOptions>>();
            app.UseRequestLocalization(options.Value);

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                   name: "error",
                   pattern: "error",
                   defaults: new { controller = "Home", action = "Error", id = "" });
                
                endpoints.MapControllerRoute(
                   name: "not-found",
                   pattern: "not-found",
                   defaults: new { controller = "Home", action = "NotFound", id = "" });

                endpoints.MapControllerRoute(
                   name: "user-management",
                   pattern: "user-management",
                   defaults: new { controller = "User", action = "ManageUser", id = "" });
                
                endpoints.MapControllerRoute(
                   name: "manage-timesheet",
                   pattern: "manage-timesheet",
                   defaults: new { controller = "Timesheet", action = "ManageTimesheet", id = "" });

                endpoints.MapControllerRoute(
                   name: "manage-activitytype",
                   pattern: "manage-activitytype",
                   defaults: new { controller = "Activity", action = "ActivityType" });

                endpoints.MapControllerRoute(
                   name: "manage-activity",
                   pattern: "manage-activity",
                   defaults: new { controller = "Activity", action = "Activity" });

                endpoints.MapControllerRoute(
                   name: "add-timesheet",
                   pattern: "add-timesheet",
                   defaults: new { controller = "Timesheet", action = "Timesheet" });

                endpoints.MapControllerRoute(
                   name: "login",
                   pattern: "login",
                   defaults: new { controller = "Account", action = "Login", id = "" });

                endpoints.MapControllerRoute(
                   name: "register",
                   pattern: "register",
                   defaults: new { controller = "Account", action = "Register", id = "" });

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Timesheet}/{action=Timesheet}/{id?}");
            });
        }
    }
}
