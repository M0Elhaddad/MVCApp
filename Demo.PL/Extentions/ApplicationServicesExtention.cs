using Demo.BLL;
using Demo.BLL.Interfaces;
using Demo.BLL.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Demo.PL.Extentions
{
    public static class ApplicationServicesExtention
    {
        public static IServiceCollection AddApplicationService(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            //services.AddScoped<IDepartmentRepository, DepartmentRepository>();

            //services.AddScoped<IEmployeeRepository, EmployeeRepository>();

            return services;
        }
    }
}
