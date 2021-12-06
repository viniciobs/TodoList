using Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.DataConfigurations
{
	public class UsersContextConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> entity)
		{
			entity.ToTable("Users");

			entity.HasKey(e => e.Id);

			entity.Property(e => e.CreatedAt)
					.HasColumnType("datetime")
					.IsRequired();

			entity.Property(e => e.Name)
				.IsRequired()
				.HasMaxLength(30)
				.IsUnicode(false);

			entity.Property(e => e.Login)
				.IsRequired()
				.HasMaxLength(30)
				.IsUnicode(false);

			entity.Property(e => e.Role)
				.IsRequired()
				.HasDefaultValue(UserRole.Normal);

			entity.Property(e => e.Password)
				.IsRequired()
				.HasMaxLength(20)
				.IsUnicode(false);

			entity.Property(e => e.IsActive)
				.IsRequired()
				.HasDefaultValue(true);

			entity.HasIndex(e => e.Login).HasDatabaseName("IX_User_Login").IsUnique();
		}
	}
}