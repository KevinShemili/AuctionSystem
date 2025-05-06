using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class BidEntityConfiguration : IEntityTypeConfiguration<Bid> {
		public void Configure(EntityTypeBuilder<Bid> builder) {

			builder.Property(p => p.Amount)
				   .HasPrecision(18, 2);

			builder.HasOne(x => x.Auction)
				   .WithMany(x => x.Bids)
				   .HasForeignKey(x => x.AuctionId);

			builder.HasOne(x => x.Bidder)
				   .WithMany(x => x.Bids)
				   .HasForeignKey(x => x.BidderId);

			builder.HasMany(x => x.Transactions)
				   .WithOne(x => x.Bid)
				   .HasForeignKey(x => x.BidId);
		}
	}
}
