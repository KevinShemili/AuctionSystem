using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class BidEntityConfiguration : IEntityTypeConfiguration<Bid> {
		public void Configure(EntityTypeBuilder<Bid> builder) {

			builder.ToTable("Bids");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.Amount)
				   .HasPrecision(18, 2)
				   .IsRequired();

			builder.Property(x => x.IsWinningBid)
				   .HasDefaultValue(false);

			builder.HasOne(x => x.Auction)
				   .WithMany(x => x.Bids)
				   .HasForeignKey(x => x.AuctionId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasOne(x => x.Bidder)
				   .WithMany(x => x.Bids)
				   .HasForeignKey(x => x.BidderId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.Transactions)
				   .WithOne(x => x.Bid)
				   .HasForeignKey(x => x.BidId)
				   .IsRequired(false)
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
