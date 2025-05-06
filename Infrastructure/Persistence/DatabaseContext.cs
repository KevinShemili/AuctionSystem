using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence {
	public class DatabaseContext : DbContext {

		public DatabaseContext(DbContextOptions<DatabaseContext> options)
			: base(options) {
		}

		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
		public DbSet<UserToken> UserTokens { get; set; }
		public DbSet<Auction> Auctions { get; set; }
		public DbSet<AuctionImage> AuctionImages { get; set; }
		public DbSet<Bid> Bids { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<WalletTransaction> WalletTransactions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			base.OnModelCreating(modelBuilder);
			modelBuilder.SoftDeleteFilter();
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
		}
	}
}
