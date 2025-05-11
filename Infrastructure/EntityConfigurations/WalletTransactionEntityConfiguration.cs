using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class WalletTransactionEntityConfiguration : IEntityTypeConfiguration<WalletTransaction> {
		public void Configure(EntityTypeBuilder<WalletTransaction> builder) {

			builder.ToTable("WalletTransactions");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.Amount)
				   .HasPrecision(18, 2)
				   .IsRequired();

			builder.Property(x => x.TransactionType)
				   .IsRequired();

			builder.HasOne(x => x.Wallet)
				   .WithMany(x => x.Transactions)
				   .HasForeignKey(x => x.WalletId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(x => x.Bid)
				   .WithMany(x => x.Transactions)
				   .HasForeignKey(x => x.BidId)
				   .IsRequired(false)
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
