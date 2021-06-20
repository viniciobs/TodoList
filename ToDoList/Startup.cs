using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Converters;
using Repository;
using Repository.Interfaces;
using System.Linq;
using ToDoList.UI.Configurations.ServicesConfigurations;

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
				options.SwaggerEndpoint("/swagger/Accounts/swagger.json", "Accounts");
				options.SwaggerEndpoint("/swagger/Users/swagger.json", "Users");
			});

			app.UseHttpsRedirection();
			app.UseRouting();
			app.UseAuthentication();
			app.UseAuthorization();

			app.UseResponseCompression();

			app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
		}

		#endregion Methods
	}
}