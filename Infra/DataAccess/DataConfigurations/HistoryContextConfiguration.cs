using Domains;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataAccess.DataConfigurations
{
    public class HistoryContextConfiguration : IEntityTypeConfiguration<History>
    {
        public void Configure(EntityTypeBuilder<History> entity)
        {
            entity.ToTable("History");

            entity.HasNoKey();

            entity.Property(e => e.UserId)
                .IsRequired()
                .HasColumnType("uniqueidentifier");

            entity.Property(e => e.DateTime)
                .HasColumnType("datetime")
                .IsRequired();

            entity.Property(e => e.Action)
                .IsRequired()
                .HasColumnType("int");

            entity.Property(e => e.Content)
                .IsRequired(false);
        }
    }
}