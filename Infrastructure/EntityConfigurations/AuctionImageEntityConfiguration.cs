﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class AuctionImageEntityConfiguration : IEntityTypeConfiguration<AuctionImage> {
		public void Configure(EntityTypeBuilder<AuctionImage> builder) {

			builder.ToTable("AuctionImages");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.FilePath)
				   .IsRequired()
				   .HasMaxLength(512);

			builder.HasOne(x => x.Auction)
				   .WithMany(x => x.Images)
				   .HasForeignKey(x => x.AuctionId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
