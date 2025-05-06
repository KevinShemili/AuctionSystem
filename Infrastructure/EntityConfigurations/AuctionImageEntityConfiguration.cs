using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class AuctionImageEntityConfiguration : IEntityTypeConfiguration<AuctionImage> {
		public void Configure(EntityTypeBuilder<AuctionImage> builder) {

			builder.HasOne(x => x.Auction)
				   .WithMany(x => x.Images)
				   .HasForeignKey(x => x.AuctionId);
		}
	}
}
