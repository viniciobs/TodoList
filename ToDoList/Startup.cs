using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToDoList.UI.Configurations.ServicesConfigurations;
using Newtonsoft.Json.Converters;
using Repository.Interfaces;
using Repository;
using ToDoList.UI.Configurations;

namespace ToDoList.UI
{
	public class Startup
	{
		#region Fields

		private IConfiguration configuration { get; }

		#endregion Fields

		#region Constructor

		public Startup(IConfiguration configuration)
		{
			this.configuration = configuration;
		}

		#endregion Constructor

		#region Methods

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddDbContext<DataAccess.ApplicationContext>(
				options => options.UseSqlServer(configuration.GetConnectionString("ToDoListDB"))
			);

			services.AddControllers().AddNewtonsoftJson(x => x.SerializerSettings.Converters.Add(new StringEnumConverter()));
			services.AddHttpContextAccessor();

			services.AddScoped<IUserRepository, UserRepository>();

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
			app.UseSwaggerUI(x => { x.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"); });

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}

		#endregion Methods
	}
}