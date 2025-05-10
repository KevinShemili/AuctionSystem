using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class WalletEntityConfiguration : IEntityTypeConfiguration<Wallet> {
		public void Configure(EntityTypeBuilder<Wallet> builder) {

			builder.Property(x => x.Balance)
				   .HasPrecision(18, 2);

			builder.Property(x => x.FrozenBalance)
				   .HasPrecision(18, 2);

			builder.HasOne(x => x.User)
				   .WithOne(x => x.Wallet)
				   .HasForeignKey<Wallet>(x => x.UserId)
				   .IsRequired(false); // Admins dont have wallet

			builder.HasMany(x => x.Transactions)
				   .WithOne(x => x.Wallet)
				   .HasForeignKey(x => x.WalletId);
		}
	}
}
