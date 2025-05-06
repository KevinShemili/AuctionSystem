using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class WalletTransactionEntityConfiguration : IEntityTypeConfiguration<WalletTransaction> {
		public void Configure(EntityTypeBuilder<WalletTransaction> builder) {

			builder.Property(x => x.Amount)
				   .HasPrecision(18, 2);

			builder.HasOne(x => x.Wallet)
				   .WithMany(x => x.Transactions)
				   .HasForeignKey(x => x.WalletId);

			builder.HasOne(x => x.Bid)
				   .WithMany(x => x.Transactions)
				   .HasForeignKey(x => x.BidId);
		}
	}
}
