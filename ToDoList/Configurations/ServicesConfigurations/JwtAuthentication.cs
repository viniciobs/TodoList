using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ToDoList.UI.Configurations.ServicesConfigurations
{
	public static class JwtAuthentication
	{
		public static void AddJwtAuthentication(this IServiceCollection services, IConfigurationSection authenticationSection)
		{
			services.Configure<Authentication>(authenticationSection);
			var appSettings = authenticationSection.Get<Authentication>();

			services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
			{
				options.RequireHttpsMetadata = false;
				options.SaveToken = true;
				options.TokenValidationParameters = new TokenValidationParameters()
				{
					ValidateIssuer = false,
					ValidateAudience = false,
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.Secret))
				};
			});
		}
	}
}