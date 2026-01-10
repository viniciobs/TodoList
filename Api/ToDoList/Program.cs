using DataAccess;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ToDoList.UI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var app = CreateHostBuilder(args).Build();

			using var scope = app.Services.CreateScope();			
			var db = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
			db.Database.Migrate();		

			app.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder
						.ConfigureKestrel(options =>
						{
							options.ListenLocalhost(5000);
							options.ListenLocalhost(5001, listenOptions =>
							{
								listenOptions.UseHttps();
							});
						})
						.UseStartup<Startup>();
				});
	}
}