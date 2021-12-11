using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Task = Domains.User.Task;

namespace DataAccess.DataConfigurations
{
	public class TasksContextConfiguration : IEntityTypeConfiguration<Task>
	{
		public void Configure(EntityTypeBuilder<Task> entity)
		{
			entity.ToTable("Tasks");

			entity.HasKey(e => e.Id);

			entity.Property(e => e.CompletedAt)
				.HasColumnType("datetime");

			entity.Property(e => e.Description)
				.IsRequired()
				.HasMaxLength(255)
				.IsUnicode(false);

			entity.Property(e => e.CreatedAt)
				.IsRequired();

			entity.HasOne(e => e.CreatorUser)
				.WithMany(e => e.CreatedTasks)
				.HasForeignKey(e => e.CreatorUserId)
				.HasConstraintName("FK_Task_CreatorUser")
				.OnDelete(DeleteBehavior.NoAction);

			entity.HasOne(e => e.TargetUser)
				.WithMany(e => e.TargetTasks)
				.HasForeignKey(e => e.TargetUserId)
				.HasConstraintName("FK_Task_TargetUseor")
			.OnDelete(DeleteBehavior.NoAction);
		}
	}
}