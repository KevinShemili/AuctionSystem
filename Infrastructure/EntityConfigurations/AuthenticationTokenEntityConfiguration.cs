using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class AuthenticationTokenEntityConfiguration : IEntityTypeConfiguration<AuthenticationToken> {
		public void Configure(EntityTypeBuilder<AuthenticationToken> builder) {

			builder.ToTable("AuthenticationTokens");

			builder.HasKey(x => x.Id);

			builder.Property(x => x.DateCreated)
				   .HasDefaultValueSql("NOW() AT TIME ZONE 'UTC'");

			builder.Property(x => x.DateUpdated)
				   .IsRequired(false);

			builder.Property(x => x.RefreshToken)
				   .IsRequired()
				   .HasMaxLength(512);

			builder.Property(x => x.AccessToken)
				   .IsRequired()
				   .HasMaxLength(2048);

			builder.Property(x => x.Expiry)
				   .IsRequired();

			builder.HasOne(x => x.User)
				   .WithMany(x => x.AuthenticationTokens)
				   .HasForeignKey(x => x.UserId)
				   .IsRequired()
				   .OnDelete(DeleteBehavior.NoAction);
		}
	}
}
