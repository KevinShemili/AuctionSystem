using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence {

	// DbContext is responsible for interacting with the database
	public class DatabaseContext : DbContext {

		// Receive DbContextOptions, usually configurations for the database provider
		// (SQL Server, PostgreSQL), connection string, etc.
		public DatabaseContext(DbContextOptions<DatabaseContext> options)
			: base(options) {
		}

		// Each DbSet<T> represents a table in the database,
		// and is mapped to a corresponding entity class.
		public DbSet<User> Users { get; set; }
		public DbSet<Role> Roles { get; set; }
		public DbSet<Permission> Permissions { get; set; }
		public DbSet<RolePermission> RolePermissions { get; set; }
		public DbSet<UserRole> UserRoles { get; set; }
		public DbSet<AuthenticationToken> AuthenticationTokens { get; set; }
		public DbSet<VerificationToken> VerificationTokens { get; set; }
		public DbSet<Auction> Auctions { get; set; }
		public DbSet<AuctionImage> AuctionImages { get; set; }
		public DbSet<Bid> Bids { get; set; }
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<WalletTransaction> WalletTransactions { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder) {

			// Call base implementation first to allow EF Core to set up
			base.OnModelCreating(modelBuilder);

			modelBuilder.SoftDeleteFilter();

			// Automatically applies all IEntityTypeConfiguration<> implementations found in the assembly.
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
		}
	}
}
