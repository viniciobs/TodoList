using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

namespace ToDoList.UI.Configurations.ServicesConfigurations
{
    public static class Swagger
    {
        public static string ACCOUNTS = "accounts";
        public static string USERS = "users";
        public static string TASKS = "tasks";
        public static string TASK_COMMENTS = "task-comments";

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGenNewtonsoftSupport();

            var contact = new OpenApiContact
            {
                Name = "Vinício",
                Email = "vinicio.barreto.santos@gmail.com",
                Url = new Uri("https://www.linkedin.com/in/vinicio-barreto-santos/")
            };

            var version = "1";

            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc(ACCOUNTS, new OpenApiInfo
                {
                    Version = version,
                    Title = "Accounts API",
                    Description = "The API provides accounts management",
                    Contact = contact
                });

                x.SwaggerDoc(USERS, new OpenApiInfo
                {
                    Version = version,
                    Title = "Users API",
                    Description = "The API allows users management",
                    Contact = contact
                });

                x.SwaggerDoc(TASKS, new OpenApiInfo
                {
                    Version = version,
                    Title = "Tasks API",
                    Description = "The API allows tasks management",
                    Contact = contact
                });

                x.SwaggerDoc(TASK_COMMENTS, new OpenApiInfo
                {
                    Version = version,
                    Title = "Tasks comments API",
                    Description = "The API allows tasks comments management",
                    Contact = contact
                });

                x.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Authenticate and enter \"Bearer <YOUR_JWT>\"",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                x.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                x.IncludeXmlComments(xmlPath);
            });
        }

        public static void ConfigureSwaggerEndpoints(this IApplicationBuilder app)
        {
            string getEndoint(string path)
            {
                return $"/swagger/{path}/swagger.json";
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint(getEndoint("accounts"), "Accounts");
                options.SwaggerEndpoint(getEndoint("users"), "Users");
                options.SwaggerEndpoint(getEndoint("tasks"), "Tasks");
                options.SwaggerEndpoint(getEndoint("task-comments"), "Task comments");
            });
        }
    }
}