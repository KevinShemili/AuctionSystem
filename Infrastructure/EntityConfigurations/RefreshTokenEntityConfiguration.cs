using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class RefreshTokenEntityConfiguration : IEntityTypeConfiguration<AuthenticationToken> {
		public void Configure(EntityTypeBuilder<AuthenticationToken> builder) {
			builder.HasOne(x => x.User)
				   .WithMany(x => x.RefreshTokens)
				   .HasForeignKey(x => x.UserId);
		}
	}
}
