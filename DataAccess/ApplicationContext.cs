using Microsoft.EntityFrameworkCore;
using Domains;
using Task = Domains.User.Task;
using TaskComment = Domains.User.Task.TaskComment;

namespace DataAccess
{
	public class ApplicationContext : DbContext
	{
		public DbSet<User> User { get; set; }
		public DbSet<Task> Task { get; set; }
		public DbSet<TaskComment> TaskComment { get; set; }
		public DbSet<History> History { get; set; }

		public ApplicationContext(DbContextOptions<ApplicationContext> options)
			: base(options)
		{ }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
		}
	}
}