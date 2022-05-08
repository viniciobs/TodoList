using DataAccess;
using Microsoft.Extensions.DependencyInjection;
using Repository;
using Repository._Commom;
using Repository.Interfaces;
using Repository.Interfaces_Commom;
using ToDoList.API.Services.TokenGenerator;
using ToDoList.API.Services.TokenGenerator.Interfaces;

namespace ToDoList.API.Configurations.ServicesConfigurations
{
    public static class Services
    {
        public static void ConfigureServices(this IServiceCollection services, string connectionString)
        {
            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();
            services.AddScoped<IPaginationRepository, PaginationRepository>();
            services.AddScoped<IHistoryRepository>(x => new HistoryRepository(x.GetRequiredService<ApplicationContext>(), connectionString));
            services.AddScoped<ITokenGenerator, TokenGenerator>();
        }
    }
}