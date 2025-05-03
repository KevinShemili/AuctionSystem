using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class PermissionEntityConfiguration : IEntityTypeConfiguration<Permission> {
		public void Configure(EntityTypeBuilder<Permission> builder) {
			builder.HasMany(x => x.Roles)
				   .WithMany(x => x.Permissions)
				   .UsingEntity<RolePermission>(x => x.HasKey(x => x.Id));

			// Seed Data
			builder.HasData(SeedData.Permissions);
		}
	}
}
