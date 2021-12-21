using DAL;
using DAL.Activities;
using DAL.User;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class RepositoryDependancyInjection
    {
        public static IServiceCollection AddDALDependency(this IServiceCollection services)
        {
            services.AddTransient<IActivityDAL, ActivityDAL>();
            services.AddTransient<ITimesheetDAL, TimesheetDAL>();
            services.AddTransient<IUserDAL, UserDAL>();

            return services;
        }
    }
}
