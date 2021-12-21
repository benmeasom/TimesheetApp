using BLL.Activity;
using BLL.User;
using Microsoft.Extensions.DependencyInjection;

namespace BLL
{
    public static class ServiceDependancyInjection
    {
        public static IServiceCollection AddBLLDependency(this IServiceCollection services)
        {
            services.AddTransient<IActivityBLL, ActivityBLL>();
            services.AddTransient<ITimesheetBLL, TimesheetBLL>();
            services.AddTransient<IUserBLL, UserBLL>();

            return services;
        }
    }
}
