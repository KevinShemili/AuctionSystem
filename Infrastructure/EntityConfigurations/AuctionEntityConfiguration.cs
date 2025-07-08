using Domain.Entities;
using Infrastructure.EntityConfigurations.Seeds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class AuctionEntityConfiguration : IEntityTypeConfiguration<Auction> {
		public void Configure(EntityTypeBuilder<Auction> builder) {

			builder.ToTable("Auctions");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
			   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.Name)
				   .IsRequired()
				   .HasMaxLength(200);

			builder.Property(x => x.Description)
				   .IsRequired(false)
				   .HasMaxLength(2000);

			builder.Property(x => x.BaselinePrice)
				   .HasPrecision(18, 2)
				   .IsRequired();

			builder.Property(x => x.StartTime)
				   .IsRequired();

			builder.Property(x => x.EndTime)
				   .IsRequired();

			builder.Property(x => x.Status)
				   .IsRequired();

			builder.Property(x => x.ForceClosedBy)
				   .IsRequired(false);

			builder.Property(x => x.ForceClosedReason)
				   .IsRequired(false)
				   .HasMaxLength(1000);

			builder.HasOne(x => x.Seller)
				   .WithMany(x => x.Auctions)
				   .HasForeignKey(x => x.SellerId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.Bids)
				   .WithOne(x => x.Auction)
				   .HasForeignKey(x => x.AuctionId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasMany(x => x.Images)
				   .WithOne(x => x.Auction)
				   .HasForeignKey(x => x.AuctionId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);

			builder.HasData(SeedData.Auctions);
		}
	}
}
