using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class New_SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "UserRoles",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("83940773-f701-4dcb-9e3e-2e8348989703"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1667e160-695c-4be8-b7a0-5e2a20de381f"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { "View system roles available.", "role.view", "View Role" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("997ac6b9-75bc-4889-8cff-656ab45e954a"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { "Be able to assign roles to a user. Be able to ban a user.", "user.edit", "Edit User" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ac2a6499-f569-4dae-9eea-2fea12859abf"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { "View the details of a user in the system.", "user.view", "View User" });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d99f646b-3e0f-49ca-a1a5-752464939f0a"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { "Be able to create other administrators.", "user.create", "Create User" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "DateCreated", "DateUpdated", "Description", "IsDeleted", "Key", "Name" },
                values: new object[,]
                {
                    { new Guid("07bd98b0-0c87-4926-b001-5bee0b5fcb1c"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to delete an auction.", false, "auction.delete", "Delete Auction" },
                    { new Guid("63291f30-83ab-4786-9469-f003ffadb39d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to view user's wallets & transactions.", false, "wallet.view", "View Wallet" },
                    { new Guid("939c7ba5-03c2-4779-8ce0-9fca2ab45375"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to force-pause an auction.", false, "auction.edit", "Edit Auction" },
                    { new Guid("9409c52e-685e-4554-acb1-d77b91571a6f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to assign permissions to a role.", false, "role.edit", "Edit Role" },
                    { new Guid("beabb24d-8666-4b11-9b40-59c6024fc7e4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "View auctions in the system in detail.", false, "auction.view", "View Auction" },
                    { new Guid("f0f9bf98-4b47-453d-8ef4-c51bbe644b56"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Create a new role for the system.", false, "role.create", "Create Role" }
                });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2193d4c2-3d86-4059-a642-e5338c51167a"),
                column: "Name",
                value: "SuperAdmin");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "CFE7ECDA6446E08AB0D53B05A80CA0C8345C3116EBFC979C4BBB8506906231BCE541E5A58A3A2DEFA02DA8A6FDCAD9DBC788E59642025011CD22C12BCF570ACA", "2A922A1E3E3AE6619430EDBA4CF6E5DED96CB3CBE4C01FD55F4DF05B5C3929E9CFD552FB90D148D8277AEA0169AEB0B9C1D5483C46BD06F0FB033649989635FD" });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "AssignedBy", "AssignedByName", "DateCreated", "DateUpdated", "IsDeleted", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("55555555-5555-5555-5555-555555555555"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("f0f9bf98-4b47-453d-8ef4-c51bbe644b56"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("66666666-6666-6666-6666-666666666666"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("9409c52e-685e-4554-acb1-d77b91571a6f"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("beabb24d-8666-4b11-9b40-59c6024fc7e4"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("88888888-8888-8888-8888-888888888888"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("939c7ba5-03c2-4779-8ce0-9fca2ab45375"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("99999999-9999-9999-9999-999999999999"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("07bd98b0-0c87-4926-b001-5bee0b5fcb1c"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("63291f30-83ab-4786-9469-f003ffadb39d"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("66666666-6666-6666-6666-666666666666"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("77777777-7777-7777-7777-777777777777"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("88888888-8888-8888-8888-888888888888"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("99999999-9999-9999-9999-999999999999"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumn: "Id",
                keyValue: new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("07bd98b0-0c87-4926-b001-5bee0b5fcb1c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("63291f30-83ab-4786-9469-f003ffadb39d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("939c7ba5-03c2-4779-8ce0-9fca2ab45375"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("9409c52e-685e-4554-acb1-d77b91571a6f"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("beabb24d-8666-4b11-9b40-59c6024fc7e4"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("f0f9bf98-4b47-453d-8ef4-c51bbe644b56"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("1667e160-695c-4be8-b7a0-5e2a20de381f"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { null, "user.create", "Create new user." });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("997ac6b9-75bc-4889-8cff-656ab45e954a"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { null, "role.assign", "Assign role to user." });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("ac2a6499-f569-4dae-9eea-2fea12859abf"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { null, "permission.assign", "Assign permission to role." });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("d99f646b-3e0f-49ca-a1a5-752464939f0a"),
                columns: new[] { "Description", "Key", "Name" },
                values: new object[] { null, "role.create", "Create new role." });

            migrationBuilder.UpdateData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: new Guid("2193d4c2-3d86-4059-a642-e5338c51167a"),
                column: "Name",
                value: "administrator");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "DateCreated", "DateUpdated", "Description", "IsDeleted", "Name" },
                values: new object[] { new Guid("83940773-f701-4dcb-9e3e-2e8348989703"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "user" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "E9D28C04BB18622A3B4AF098D998E09D19C3921DC0622F6D9D102B585C9C1CEF392B31541B91A83783E1A276C4DBC8A3794203F95A22D637C8780FB0BD8F02F0", "8B6307059EED0CFBDE9203D33BD28BB072574CF1162F1A945437270D3016D28E6739516A2356C9EAD4D239883668F3089CF9CFB8D12780C581736519FD28C7A2" });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "AssignedBy", "AssignedByName", "DateCreated", "DateUpdated", "IsDeleted", "RoleId", "UserId" },
                values: new object[] { new Guid("66666666-6666-6666-6666-666666666666"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("83940773-f701-4dcb-9e3e-2e8348989703"), new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e") });
        }
    }
}
