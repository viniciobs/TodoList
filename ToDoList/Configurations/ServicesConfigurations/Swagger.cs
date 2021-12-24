using Microsoft.Extensions.Configuration;
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

		public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSwaggerGenNewtonsoftSupport();

			var contact = new OpenApiContact
			{
				Name = "Vinício",
				Email = "vinicio.barreto.santos@gmail.com",
				Url = new Uri("https://www.linkedin.com/in/vinicio-barreto-santos/")
			};

			services.AddSwaggerGen(x =>
			{
				x.SwaggerDoc(ACCOUNTS, new OpenApiInfo
				{
					Version = "1",
					Title = "Accounts API",
					Description = "The API provides accounts management",
					Contact = contact
				});

				x.SwaggerDoc(USERS, new OpenApiInfo
				{
					Version = "1",
					Title = "Users API",
					Description = "The API allows users management",
					Contact = contact
				});

				x.SwaggerDoc(TASKS, new OpenApiInfo
				{
					Version = "1",
					Title = "Tasks API",
					Description = "The API allows tasks management",
					Contact = contact
				});

				x.SwaggerDoc(TASK_COMMENTS, new OpenApiInfo
				{
					Version = "1",
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
	}
}