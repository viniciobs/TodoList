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
		public static void AddSwagger(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddSwaggerGenNewtonsoftSupport();

			services.AddSwaggerGen(x =>
			{
				x.SwaggerDoc("Users", new OpenApiInfo
				{
					Version = "1",
					Title = "USers API",
					Description = "The API allows users management",
					Contact = new OpenApiContact
					{
						Name = "Vinício",
						Email = "vinicio.barreto.santos@gmail.com",
						Url = new Uri("https://www.linkedin.com/in/vinicio-barreto-santos/")
					}
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