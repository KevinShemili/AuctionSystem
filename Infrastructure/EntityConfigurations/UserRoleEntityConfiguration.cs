using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class UserRoleEntityConfiguration : IEntityTypeConfiguration<UserRole> {
		public void Configure(EntityTypeBuilder<UserRole> builder) {

			builder.ToTable("UserRoles");

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

			builder.HasOne(x => x.User)
				   .WithMany(x => x.UserRoles)
				   .HasForeignKey(x => x.UserId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(x => x.Role)
				   .WithMany(x => x.UserRoles)
				   .HasForeignKey(x => x.RoleId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			// Seed Data
			builder.HasData(SeedData.UserRoles);
		}
	}
}
