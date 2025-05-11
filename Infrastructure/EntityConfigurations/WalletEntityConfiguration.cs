using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class WalletEntityConfiguration : IEntityTypeConfiguration<Wallet> {
		public void Configure(EntityTypeBuilder<Wallet> builder) {

			builder.ToTable("Wallets");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.Balance)
				   .HasPrecision(18, 2)
				   .HasDefaultValue(0)
				   .IsRequired();

			builder.Property(x => x.FrozenBalance)
				   .HasPrecision(18, 2)
				   .HasDefaultValue(0)
				   .IsRequired();

			builder.HasOne(x => x.User)
				   .WithOne(x => x.Wallet)
				   .HasForeignKey<Wallet>(x => x.UserId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.Transactions)
				   .WithOne(x => x.Wallet)
				   .HasForeignKey(x => x.WalletId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
