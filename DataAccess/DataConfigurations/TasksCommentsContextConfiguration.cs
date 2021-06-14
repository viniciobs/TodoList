using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Comment = Domains.User.Task.TaskComment;

namespace DataAccess.DataConfigurations
{
	public class TasksCommentsContextConfiguration : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> entity)
		{
			entity.ToTable("TasksComments");

			entity.HasKey(e => e.Id);

			entity.Property(e => e.CreatedAt)
				.HasColumnType("datetime")
				.IsRequired();

			entity.Property(e => e.Text)
				.IsRequired()
				.IsUnicode(false)
				.HasMaxLength(255);

			entity.HasOne(e => e.Task)
				.WithMany(e => e.Comments)
				.HasForeignKey(e => e.TaskId)
				.HasConstraintName("FK_Comment_Task")
				.OnDelete(DeleteBehavior.Cascade);

			entity.HasOne(e => e.CreatedBy)
				.WithMany(e => e.TaskComments)
				.HasForeignKey(e => e.CreatedByUserId)
				.HasConstraintName("FK_Comment_CreatedBy")
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}