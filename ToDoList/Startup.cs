using DataAccess;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Repository;
using Repository._Commom;
using Repository.Interfaces;
using Repository.Interfaces_Commom;
using System.Linq;
using ToDoList.UI.Configurations.ServicesConfigurations;

namespace ToDoList.UI
{
    public class Startup
    {
        private IConfiguration configuration { get; }

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            string connectionString = configuration.GetConnectionString("ToDoListDB");

            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowAnyOrigin();
                });
            });

            services.AddDbContext<DataAccess.ApplicationContext>(
                options => options.UseSqlServer(connectionString)
            );

            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
                options.Providers.Add<GzipCompressionProvider>();
                options.MimeTypes = ResponseCompressionDefaults.MimeTypes.Concat(new[] { "application/json" });
            });

            services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new StringEnumConverter()));
            services.AddHttpContextAccessor();

            services.AddScoped<IAccountRepository, AccountRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<ITaskCommentRepository, TaskCommentRepository>();
            services.AddScoped<IPaginationRepository, PaginationRepository>();
            services.AddScoped<IHistoryRepository>(x => new HistoryRepository(x.GetRequiredService<ApplicationContext>(), connectionString));

            services.AddJwtAuthentication(configuration.GetSection("Authentication"));
            services.AddSwagger(configuration);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint($"/swagger/{Swagger.ACCOUNTS}/swagger.json", "Accounts");
                options.SwaggerEndpoint($"/swagger/{Swagger.USERS}/swagger.json", "Users");
                options.SwaggerEndpoint($"/swagger/{Swagger.TASKS}/swagger.json", "Tasks");
                options.SwaggerEndpoint($"/swagger/{Swagger.TASK_COMMENTS}/swagger.json", "Task comments");
            });

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseCors();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseResponseCompression();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}