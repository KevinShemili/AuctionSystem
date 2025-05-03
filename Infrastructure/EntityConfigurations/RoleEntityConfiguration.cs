using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class RoleEntityConfiguration : IEntityTypeConfiguration<Role> {
		public void Configure(EntityTypeBuilder<Role> builder) {
			builder.HasMany(x => x.Users)
				   .WithMany(x => x.Roles)
				   .UsingEntity<UserRole>(x => x.HasKey(x => x.Id));

			builder.HasMany(x => x.Permissions)
				   .WithMany(x => x.Roles)
				   .UsingEntity<RolePermission>(x => x.HasKey(x => x.Id));

			// Seed Data
			builder.HasData(SeedData.Roles);
		}
	}
}
