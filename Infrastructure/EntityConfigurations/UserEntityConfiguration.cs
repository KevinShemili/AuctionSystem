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

			builder.HasMany(x => x.AuthenticationTokens)
				   .WithOne(x => x.User)
				   .HasForeignKey(x => x.UserId);

			builder.HasMany(x => x.UserTokens)
				   .WithOne(x => x.User)
				   .HasForeignKey(x => x.UserId);

			builder.HasOne(x => x.Wallet)
				   .WithOne(x => x.User)
				   .HasForeignKey<Wallet>(x => x.UserId);

			builder.HasMany(x => x.Auctions)
				   .WithOne(x => x.Seller)
				   .HasForeignKey(x => x.SellerId);

			builder.HasMany(x => x.Bids)
				   .WithOne(x => x.Bidder)
				   .HasForeignKey(x => x.BidderId);

			// Seed Data
			builder.HasData(SeedData.Users);
		}
	}
}
