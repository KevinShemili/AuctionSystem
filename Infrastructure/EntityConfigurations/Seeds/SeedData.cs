using Application.Common.Tools.Passwords;
using Application.PermissionKeys;
using Domain.Entities;

// Static Time (DateTime) and static UUIDs (Guid) have been 
// used in order to prevent EF Drift, which would occur due to
// DateTime.UtcNow and Guid.NewGuid() generating a new value 
// each time the code is run.

namespace Infrastructure.EntityConfigurations.Seeds {
	public class SeedData {

		private static readonly DateTime SeedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		#region Permissions
		public static readonly Permission ViewUser = new Permission {
			Id = Guid.Parse("ac2a6499-f569-4dae-9eea-2fea12859abf"),
			Key = PermissionKeys.ViewUser,
			Name = "View User",
			Description = "View the details of a user in the system.",
			DateCreated = SeedDate
		};
		public static readonly Permission EditUser = new Permission {
			Id = Guid.Parse("997ac6b9-75bc-4889-8cff-656ab45e954a"),
			Key = PermissionKeys.EditUser,
			Name = "Edit User",
			Description = "Be able to assign roles to a user. Be able to ban a user.",
			DateCreated = SeedDate
		};
		public static readonly Permission CreateUser = new Permission {
			Id = Guid.Parse("d99f646b-3e0f-49ca-a1a5-752464939f0a"),
			Key = PermissionKeys.CreateUser,
			Name = "Create User",
			Description = "Be able to create other administrators.",
			DateCreated = SeedDate
		};
		public static readonly Permission ViewRole = new Permission {
			Id = Guid.Parse("1667e160-695c-4be8-b7a0-5e2a20de381f"),
			Key = PermissionKeys.ViewRole,
			Name = "View Role",
			Description = "View system roles available.",
			DateCreated = SeedDate
		};
		public static readonly Permission CreateRole = new Permission {
			Id = Guid.Parse("f0f9bf98-4b47-453d-8ef4-c51bbe644b56"),
			Key = PermissionKeys.CreateRole,
			Name = "Create Role",
			Description = "Create a new role for the system.",
			DateCreated = SeedDate
		};
		public static readonly Permission EditRole = new Permission {
			Id = Guid.Parse("9409c52e-685e-4554-acb1-d77b91571a6f"),
			Key = PermissionKeys.EditRole,
			Name = "Edit Role",
			Description = "Be able to assign permissions to a role.",
			DateCreated = SeedDate
		};
		public static readonly Permission ViewAuction = new Permission {
			Id = Guid.Parse("beabb24d-8666-4b11-9b40-59c6024fc7e4"),
			Key = PermissionKeys.ViewAuction,
			Name = "View Auction",
			Description = "View auctions in the system in detail.",
			DateCreated = SeedDate
		};
		public static readonly Permission EditAuction = new Permission {
			Id = Guid.Parse("939c7ba5-03c2-4779-8ce0-9fca2ab45375"),
			Key = PermissionKeys.EditAuction,
			Name = "Edit Auction",
			Description = "Be able to force-pause an auction.",
			DateCreated = SeedDate
		};
		public static readonly Permission DeleteAuction = new Permission {
			Id = Guid.Parse("07bd98b0-0c87-4926-b001-5bee0b5fcb1c"),
			Key = PermissionKeys.DeleteAuction,
			Name = "Delete Auction",
			Description = "Be able to delete an auction.",
			DateCreated = SeedDate
		};
		public static readonly Permission ViewWallet = new Permission {
			Id = Guid.Parse("63291f30-83ab-4786-9469-f003ffadb39d"),
			Key = PermissionKeys.ViewWallet,
			Name = "View Wallet",
			Description = "Be able to view user's wallets & transactions.",
			DateCreated = SeedDate
		};
		public static readonly List<Permission> Permissions = new() {
			ViewUser, EditUser, CreateUser, ViewRole,
			CreateRole, EditRole, ViewAuction,
			EditAuction, DeleteAuction, ViewWallet
		};
		#endregion

		#region Roles
		public static class RoleNames {
			public const string SuperAdmin = "SuperAdmin";
		}
		public static readonly Role SuperAdmin = new Role {
			Id = Guid.Parse("2193d4c2-3d86-4059-a642-e5338c51167a"),
			Name = RoleNames.SuperAdmin,
			DateCreated = SeedDate
		};
		public static readonly List<Role> Roles = new() {
			SuperAdmin
		};
		#endregion

		#region Users
		public static readonly User Admin = new User {
			Id = Guid.Parse("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
			FirstName = "admin",
			LastName = "admin",
			Email = "admin@mail.com",
			IsEmailVerified = true,
			PasswordHash = Hasher.AdminHash,
			PasswordSalt = Hasher.AdminSalt,
			DateCreated = SeedDate
		};

		public static readonly User User1 = new User {
			Id = Guid.Parse("c75ce5c0-cf73-44be-849b-7e1de26ae992"),
			FirstName = "Kevin",
			LastName = "Shemili",
			Email = "kevinshemili5@gmail.com",
			IsEmailVerified = true,
			PasswordHash = Hasher.AdminHash,
			PasswordSalt = Hasher.AdminSalt,
			DateCreated = SeedDate
		};

		public static readonly User User2 = new User {
			Id = Guid.Parse("8884546c-45cc-496e-97b1-b7c861c3cafa"),
			FirstName = "John",
			LastName = "Johnson",
			Email = "kevin.shemili@edu.unifi.it",
			IsEmailVerified = true,
			PasswordHash = Hasher.AdminHash,
			PasswordSalt = Hasher.AdminSalt,
			DateCreated = SeedDate
		};

		public static readonly User User3 = new User {
			Id = Guid.Parse("783855e1-d39d-402a-9235-175eaf1eb472"),
			FirstName = "Adam",
			LastName = "Smith",
			Email = "kevin001sh@gmail.com",
			IsEmailVerified = true,
			PasswordHash = Hasher.AdminHash,
			PasswordSalt = Hasher.AdminSalt,
			DateCreated = SeedDate
		};

		public static readonly List<User> Users = new() { Admin, User1, User2, User3 };
		#endregion

		#region Wallets
		public static readonly Wallet User1Wallet = new Wallet {
			Id = Guid.Parse("1da693bc-9c40-4ca4-a0f4-1c5af1a9d391"),
			Balance = 1000m,
			DateCreated = SeedDate,
			UserId = User1.Id
		};

		public static readonly Wallet User2Wallet = new Wallet {
			Id = Guid.Parse("14e2427a-99f2-47d5-a02d-e565e212fc03"),
			Balance = 1000m,
			DateCreated = SeedDate,
			UserId = User2.Id
		};

		public static readonly Wallet User3Wallet = new Wallet {
			Id = Guid.Parse("aa9bf01e-3879-4ce7-8ebb-07a18818ebe7"),
			Balance = 1000m,
			DateCreated = SeedDate,
			UserId = User3.Id
		};

		public static readonly List<Wallet> Wallets = new() { User1Wallet, User2Wallet, User3Wallet };
		#endregion

		#region Auctions
		public static readonly Auction CarAuction = new Auction {
			Id = Guid.Parse("a507ebae-effd-4317-aa59-8df75282a953"),
			Name = "Mercedes C-Class 2010",
			Description = "A well-maintained Mercedes C-Class from 2010, perfect for city driving and long trips.",
			BaselinePrice = 1000m,
			DateCreated = SeedDate,
			EndTime = SeedDate.AddDays(2),
			StartTime = SeedDate,
			SellerId = User1.Id
		};

		public static readonly Auction PaintingAuction = new Auction {
			Id = Guid.Parse("19d17521-1036-418b-9911-613e607c7be4"),
			Name = "City Night Painting",
			Description = "A beautiful painting of a city at night, perfect for art lovers.",
			BaselinePrice = 300m,
			DateCreated = SeedDate,
			EndTime = SeedDate.AddDays(2),
			StartTime = SeedDate,
			SellerId = User1.Id
		};

		public static readonly Auction SneakersAuction = new Auction {
			Id = Guid.Parse("9619c8d2-52ce-4185-b5c4-06a304ae936b"),
			Name = "High End Fashion Sneakers",
			Description = "Sneakers from a high-end fashion brand, perfect for collectors.",
			BaselinePrice = 250m,
			DateCreated = SeedDate,
			EndTime = SeedDate.AddDays(2),
			StartTime = SeedDate,
			SellerId = User2.Id
		};

		public static readonly Auction LaptopAuction = new Auction {
			Id = Guid.Parse("4f32e981-8c1e-4e3c-8e47-c98122d8ed49"),
			Name = "Workstation Laptop",
			Description = "A powerful workstation laptop, ideal for professionals and gamers.",
			BaselinePrice = 850m,
			DateCreated = SeedDate,
			EndTime = SeedDate.AddDays(2),
			StartTime = SeedDate,
			SellerId = User2.Id
		};

		public static readonly List<Auction> Auctions = new() { CarAuction, PaintingAuction, SneakersAuction, LaptopAuction };
		#endregion

		#region RolePermissions
		public static readonly List<RolePermission> RolePermissions = new() {
			new RolePermission {
				Id             = Guid.Parse("11111111-1111-1111-1111-111111111111"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = ViewUser.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("22222222-2222-2222-2222-222222222222"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = EditUser.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("33333333-3333-3333-3333-333333333333"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = CreateUser.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("44444444-4444-4444-4444-444444444444"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = ViewRole.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("55555555-5555-5555-5555-555555555555"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = CreateRole.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("66666666-6666-6666-6666-666666666666"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = EditRole.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("77777777-7777-7777-7777-777777777777"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = ViewAuction.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("88888888-8888-8888-8888-888888888888"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = EditAuction.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("99999999-9999-9999-9999-999999999999"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = DeleteAuction.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			},
			new RolePermission {
				Id             = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"),
				RoleId         = SuperAdmin.Id,
				PermissionId   = ViewWallet.Id,
				AssignedByName = "system",
				DateCreated    = SeedDate
			}
		};
		#endregion

		#region UserRoles
		public static readonly List<UserRole> UserRoles = new() {
			new UserRole {
				Id            = Guid.Parse("55555555-5555-5555-5555-555555555555"),
				UserId        = Admin.Id,
				RoleId        = SuperAdmin.Id,
				AssignedByName= "system",
				DateCreated   = SeedDate
			},
		};
		#endregion
	}
}