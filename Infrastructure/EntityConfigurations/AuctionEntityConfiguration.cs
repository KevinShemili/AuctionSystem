using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class AuctionEntityConfiguration : IEntityTypeConfiguration<Auction> {
		public void Configure(EntityTypeBuilder<Auction> builder) {

			builder.Property(p => p.BaselinePrice)
				   .HasPrecision(18, 2);

			builder.HasOne(x => x.Seller)
				   .WithMany(x => x.Auctions)
				   .HasForeignKey(x => x.SellerId);

			builder.HasMany(x => x.Bids)
				   .WithOne(x => x.Auction)
				   .HasForeignKey(x => x.AuctionId);

			builder.HasMany(x => x.Images)
				   .WithOne(x => x.Auction)
				   .HasForeignKey(x => x.AuctionId);
		}
	}
}
