using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class RolePermissionEntityConfiguration : IEntityTypeConfiguration<RolePermission> {
		public void Configure(EntityTypeBuilder<RolePermission> builder) {
			builder.HasKey(x => x.Id);

			builder.HasOne(x => x.Role)
				   .WithMany(x => x.RolePermissions)
				   .HasForeignKey(x => x.RoleId);

			builder.HasOne(x => x.Permission)
				   .WithMany(x => x.RolePermissions)
				   .HasForeignKey(x => x.PermissionId);

			// Seed Data
			builder.HasData(SeedData.RolePermissions);
		}
	}
}
