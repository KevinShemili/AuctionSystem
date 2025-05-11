using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class VerificationTokenEntityConfiguration : IEntityTypeConfiguration<VerificationToken> {
		public void Configure(EntityTypeBuilder<VerificationToken> builder) {

			builder.ToTable("VerificationTokens");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.Token)
				   .IsRequired()
				   .HasMaxLength(512);

			builder.Property(x => x.Expiry)
				   .IsRequired();

			builder.Property(x => x.TokenTypeId)
				   .IsRequired();

			builder.HasOne(x => x.User)
				   .WithMany(x => x.VerificationTokens)
				   .HasForeignKey(x => x.UserId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}