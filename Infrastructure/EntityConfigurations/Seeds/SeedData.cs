using Application.Common.Tools.Hasher;
using Domain.Entities;

// Static Time (DateTime) and static UUIDs (Guid) have been 
// used in order to prevent EF Drift, which would occur due to
// DateTime.UtcNow and Guid.NewGuid() generating a new value 
// each time the code is run.

namespace Infrastructure.EntityConfigurations.Seeds {
	public class SeedData {
		public static class PermissionKeys {
			public const string AssignPermission = "permission.assign";
			public const string AssignRole = "role.assign";
			public const string CreateRole = "role.create";
			public const string CreateUser = "user.create";
		}

		private static readonly DateTime SeedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		#region Permissions
		public static readonly Permission AssignPermission = new Permission {
			Id = Guid.Parse("ac2a6499-f569-4dae-9eea-2fea12859abf"),
			Key = PermissionKeys.AssignPermission,
			Name = "Assign permission to role.",
			DateCreated = SeedDate
		};
		public static readonly Permission AssignRole = new Permission {
			Id = Guid.Parse("997ac6b9-75bc-4889-8cff-656ab45e954a"),
			Key = PermissionKeys.AssignRole,
			Name = "Assign role to user.",
			DateCreated = SeedDate
		};
		public static readonly Permission CreateRole = new Permission {
			Id = Guid.Parse("d99f646b-3e0f-49ca-a1a5-752464939f0a"),
			Key = PermissionKeys.CreateRole,
			Name = "Create new role.",
			DateCreated = SeedDate
		};
		public static readonly Permission CreateUser = new Permission {
			Id = Guid.Parse("1667e160-695c-4be8-b7a0-5e2a20de381f"),
			Key = PermissionKeys.CreateUser,
			Name = "Create new user.",
			DateCreated = SeedDate
		};
		public static readonly List<Permission> Permissions = new() {
			AssignPermission, AssignRole, CreateRole, CreateUser
		};
		#endregion

		#region Roles
		public static class RoleNames {
			public const string Administrator = "administrator";
			public const string BasicUser = "user";
		}
		public static readonly Role AdministratorRole = new Role {
			Id = Guid.Parse("2193d4c2-3d86-4059-a642-e5338c51167a"),
			Name = RoleNames.Administrator,
			DateCreated = SeedDate
		};
		public static readonly Role BasicUserRole = new Role {
			Id = Guid.Parse("83940773-f701-4dcb-9e3e-2e8348989703"),
			Name = RoleNames.BasicUser,
			DateCreated = SeedDate
		};
		public static readonly List<Role> Roles = new() {
			AdministratorRole, BasicUserRole
		};
		#endregion

		#region Users
		public static readonly User AdminUser = new User {
			Id = Guid.Parse("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
			UserName = "admin",
			Email = "admin@mail.com",
			IsEmailVerified = true,
			PasswordHash = Hasher.AdminHash,
			PasswordSalt = Hasher.AdminSalt,
			DateCreated = SeedDate
		};
		public static readonly List<User> Users = new() { AdminUser };
		#endregion

		#region RolePermissions
		public static readonly List<RolePermission> RolePermissions = new() {
			new RolePermission {
				Id            = Guid.Parse("11111111-1111-1111-1111-111111111111"),
				RoleId        = AdministratorRole.Id,
				PermissionId  = AssignPermission.Id,
				AssignedByName= "system",
				DateCreated   = SeedDate
			},
			new RolePermission {
				Id            = Guid.Parse("22222222-2222-2222-2222-222222222222"),
				RoleId        = AdministratorRole.Id,
				PermissionId  = AssignRole.Id,
				AssignedByName= "system",
				DateCreated   = SeedDate
			},
			new RolePermission {
				Id            = Guid.Parse("33333333-3333-3333-3333-333333333333"),
				RoleId        = AdministratorRole.Id,
				PermissionId  = CreateRole.Id,
				AssignedByName= "system",
				DateCreated   = SeedDate
			},
			new RolePermission {
				Id            = Guid.Parse("44444444-4444-4444-4444-444444444444"),
				RoleId        = AdministratorRole.Id,
				PermissionId  = CreateUser.Id,
				AssignedByName= "system",
				DateCreated   = SeedDate
			}
		};
		#endregion

		#region UserRoles
		public static readonly List<UserRole> UserRoles = new() {
			new UserRole {
				Id            = Guid.Parse("55555555-5555-5555-5555-555555555555"),
				UserId        = AdminUser.Id,
				RoleId        = AdministratorRole.Id,
				AssignedByName= "system",
				DateCreated   = SeedDate
			},
			new UserRole {
				Id            = Guid.Parse("66666666-6666-6666-6666-666666666666"),
				UserId        = AdminUser.Id,
				RoleId        = BasicUserRole.Id,
				AssignedByName= "system",
				DateCreated   = SeedDate
			}
		};
		#endregion
	}
}
