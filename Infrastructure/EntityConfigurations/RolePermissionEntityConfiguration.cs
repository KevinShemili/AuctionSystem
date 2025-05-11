using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class RolePermissionEntityConfiguration : IEntityTypeConfiguration<RolePermission> {
		public void Configure(EntityTypeBuilder<RolePermission> builder) {

			builder.ToTable("RolePermissions");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.AssignedBy)
				   .IsRequired(false);

			builder.Property(x => x.AssignedByName)
				   .IsRequired(false)
				   .HasMaxLength(200);

			builder.HasOne(x => x.Role)
				   .WithMany(x => x.RolePermissions)
				   .HasForeignKey(x => x.RoleId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(x => x.Permission)
				   .WithMany(x => x.RolePermissions)
				   .HasForeignKey(x => x.PermissionId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			// Seed Data
			builder.HasData(SeedData.RolePermissions);
		}
	}
}
