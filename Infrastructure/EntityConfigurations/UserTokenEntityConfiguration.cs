using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.EntityConfigurations {
	public class UserTokenEntityConfiguration : IEntityTypeConfiguration<UserToken> {
		public void Configure(EntityTypeBuilder<UserToken> builder) {
			builder.HasOne(x => x.User)
				   .WithMany(x => x.UserTokens)
				   .HasForeignKey(x => x.UserId);
		}
	}
}