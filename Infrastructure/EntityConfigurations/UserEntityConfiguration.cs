using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class UserEntityConfiguration : IEntityTypeConfiguration<User> {

		public void Configure(EntityTypeBuilder<User> builder) {
			builder.HasMany(x => x.Roles)
				   .WithMany(x => x.Users)
				   .UsingEntity<UserRole>(x => x.HasKey(x => x.Id));

			builder.HasMany(x => x.RefreshTokens)
				   .WithOne(x => x.User)
				   .HasForeignKey(x => x.UserId);

			builder.HasMany(x => x.UserTokens)
				   .WithOne(x => x.User)
				   .HasForeignKey(x => x.UserId);

			// Seed Data
			builder.HasData(SeedData.Users);
		}
	}
}
