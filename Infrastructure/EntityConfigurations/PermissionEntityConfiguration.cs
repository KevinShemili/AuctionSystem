using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class PermissionEntityConfiguration : IEntityTypeConfiguration<Permission> {
		public void Configure(EntityTypeBuilder<Permission> builder) {

			builder.ToTable("Permissions");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.Key)
				   .IsRequired()
				   .HasMaxLength(100);

			builder.Property(x => x.Name)
				   .IsRequired()
				   .HasMaxLength(200);

			builder.Property(x => x.Description)
				   .IsRequired(false)
				   .HasMaxLength(1000);

			builder.HasMany(x => x.Roles)
				   .WithMany(x => x.Permissions)
				   .UsingEntity<RolePermission>(x => x.HasKey(x => x.Id));

			// Seed Data
			builder.HasData(SeedData.Permissions);
		}
	}
}
