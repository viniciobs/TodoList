using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository.Interfaces;
using Repository.Interfaces_Commom;
using Repository.Pagination;

namespace ToDoList.API.Configurations.ServicesConfigurations
{
    public static class Services
    {
        public static void ConfigureServicesOld(this IServiceCollection services)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();
            services.AddScoped<IPaginationRepository, PaginationRepository>();
        }
    }
}