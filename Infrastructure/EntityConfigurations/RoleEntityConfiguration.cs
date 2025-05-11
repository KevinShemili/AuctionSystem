using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class RoleEntityConfiguration : IEntityTypeConfiguration<Role> {
		public void Configure(EntityTypeBuilder<Role> builder) {

			builder.ToTable("Roles");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.Name)
				   .IsRequired()
				   .HasMaxLength(100);

			builder.Property(x => x.Description)
				   .IsRequired(false)
				   .HasMaxLength(1000);

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
