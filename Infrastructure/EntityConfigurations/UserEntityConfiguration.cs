using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class UserEntityConfiguration : IEntityTypeConfiguration<User> {

		public void Configure(EntityTypeBuilder<User> builder) {

			builder.ToTable("Users");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
			   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.FirstName)
				   .IsRequired()
				   .HasMaxLength(50);

			builder.Property(x => x.LastName)
				   .IsRequired()
				   .HasMaxLength(50);

			builder.HasIndex(x => x.Email)
				   .IsUnique();

			builder.Property(x => x.Email)
				   .IsRequired()
				   .HasMaxLength(100);

			builder.Property(x => x.IsEmailVerified)
				   .HasDefaultValue(false);

			builder.Property(x => x.PasswordHash)
				   .IsRequired();

			builder.Property(x => x.PasswordSalt)
				   .IsRequired();

			builder.Property(u => u.FailedLoginTries)
				   .HasDefaultValue(0);

			builder.Property(u => u.IsBlocked)
				   .HasDefaultValue(false);

			builder.Property(u => u.BlockReason)
				   .HasMaxLength(1000);

			builder.Property(u => u.IsAdministrator)
				   .HasDefaultValue(false);

			builder.HasMany(x => x.Roles)
				   .WithMany(x => x.Users)
				   .UsingEntity<UserRole>(x => x.HasKey(x => x.Id));

			builder.HasMany(x => x.AuthenticationTokens)
				   .WithOne(x => x.User)
				   .HasForeignKey(x => x.UserId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.VerificationTokens)
				   .WithOne(x => x.User)
				   .HasForeignKey(x => x.UserId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(x => x.Wallet)
				   .WithOne(x => x.User)
				   .HasForeignKey<Wallet>(x => x.UserId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.Auctions)
				   .WithOne(x => x.Seller)
				   .HasForeignKey(x => x.SellerId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.Bids)
				   .WithOne(x => x.Bidder)
				   .HasForeignKey(x => x.BidderId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			// Seed Data
			builder.HasData(SeedData.Users);
		}
	}
}
