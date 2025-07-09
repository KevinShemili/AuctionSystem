using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DBSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Key = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsEmailVerified = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    PasswordSalt = table.Column<string>(type: "text", nullable: false),
                    FailedLoginTries = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    IsBlocked = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    BlockReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    IsAdministrator = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedByName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Auctions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    BaselinePrice = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    StartTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    ForceClosedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ForceClosedReason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    SellerId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctions_Users_SellerId",
                        column: x => x.SellerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RefreshToken = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AccessToken = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AssignedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    AssignedByName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VerificationTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Token = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    Expiry = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TokenTypeId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Wallets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Balance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    FrozenBalance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false, defaultValue: 0m),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wallets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Wallets_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuctionImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FilePath = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuctionImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuctionImages_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Bids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    IsWinningBid = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    AuctionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BidderId = table.Column<Guid>(type: "uuid", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bids_Auctions_AuctionId",
                        column: x => x.AuctionId,
                        principalTable: "Auctions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Bids_Users_BidderId",
                        column: x => x.BidderId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "WalletTransactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    TransactionType = table.Column<int>(type: "integer", nullable: false),
                    WalletId = table.Column<Guid>(type: "uuid", nullable: false),
                    BidId = table.Column<Guid>(type: "uuid", nullable: true),
                    DateCreated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "NOW() AT TIME ZONE 'UTC'"),
                    DateUpdated = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WalletTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Bids_BidId",
                        column: x => x.BidId,
                        principalTable: "Bids",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_WalletTransactions_Wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "Wallets",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "DateCreated", "DateUpdated", "Description", "IsDeleted", "Key", "Name" },
                values: new object[,]
                {
                    { new Guid("07bd98b0-0c87-4926-b001-5bee0b5fcb1c"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to delete an auction.", false, "auction.delete", "Delete Auction" },
                    { new Guid("1667e160-695c-4be8-b7a0-5e2a20de381f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "View system roles available.", false, "role.view", "View Role" },
                    { new Guid("63291f30-83ab-4786-9469-f003ffadb39d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to view user's wallets & transactions.", false, "wallet.view", "View Wallet" },
                    { new Guid("939c7ba5-03c2-4779-8ce0-9fca2ab45375"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to force-pause an auction.", false, "auction.edit", "Edit Auction" },
                    { new Guid("9409c52e-685e-4554-acb1-d77b91571a6f"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to assign permissions to a role.", false, "role.edit", "Edit Role" },
                    { new Guid("997ac6b9-75bc-4889-8cff-656ab45e954a"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to assign roles to a user. Be able to ban a user.", false, "user.edit", "Edit User" },
                    { new Guid("ac2a6499-f569-4dae-9eea-2fea12859abf"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "View the details of a user in the system.", false, "user.view", "View User" },
                    { new Guid("beabb24d-8666-4b11-9b40-59c6024fc7e4"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "View auctions in the system in detail.", false, "auction.view", "View Auction" },
                    { new Guid("d99f646b-3e0f-49ca-a1a5-752464939f0a"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Be able to create other administrators.", false, "user.create", "Create User" },
                    { new Guid("f0f9bf98-4b47-453d-8ef4-c51bbe644b56"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Create a new role for the system.", false, "role.create", "Create Role" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "DateCreated", "DateUpdated", "Description", "IsDeleted", "Name" },
                values: new object[] { new Guid("2193d4c2-3d86-4059-a642-e5338c51167a"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "SuperAdmin" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BlockReason", "DateCreated", "DateUpdated", "Email", "FirstName", "IsDeleted", "IsEmailVerified", "LastName", "PasswordHash", "PasswordSalt" },
                values: new object[,]
                {
                    { new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "admin@mail.com", "admin", false, true, "admin", "2B5B211063CE1167E4F9DC0729CFF8FAB66E94CFB6639FD00250C41ED2BA9E5B95D334BB57C467BC8444C191589DDE3266ACA2CA2AABA3972E843C6119C332E1", "F463E5D03B7BC74F0EA1E92DEE6EB9738A1D6AA706B5B901CE79F913A0F182D50E53BC818A94831B7D07FEE7AA3248218955E3393A809A1F1898FDB0B7B2D059" },
                    { new Guid("783855e1-d39d-402a-9235-175eaf1eb472"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevin001sh@gmail.com", "Adam", false, true, "Smith", "2B5B211063CE1167E4F9DC0729CFF8FAB66E94CFB6639FD00250C41ED2BA9E5B95D334BB57C467BC8444C191589DDE3266ACA2CA2AABA3972E843C6119C332E1", "F463E5D03B7BC74F0EA1E92DEE6EB9738A1D6AA706B5B901CE79F913A0F182D50E53BC818A94831B7D07FEE7AA3248218955E3393A809A1F1898FDB0B7B2D059" },
                    { new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevin.shemili@edu.unifi.it", "John", false, true, "Johnson", "2B5B211063CE1167E4F9DC0729CFF8FAB66E94CFB6639FD00250C41ED2BA9E5B95D334BB57C467BC8444C191589DDE3266ACA2CA2AABA3972E843C6119C332E1", "F463E5D03B7BC74F0EA1E92DEE6EB9738A1D6AA706B5B901CE79F913A0F182D50E53BC818A94831B7D07FEE7AA3248218955E3393A809A1F1898FDB0B7B2D059" },
                    { new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevinshemili5@gmail.com", "Kevin", false, true, "Shemili", "2B5B211063CE1167E4F9DC0729CFF8FAB66E94CFB6639FD00250C41ED2BA9E5B95D334BB57C467BC8444C191589DDE3266ACA2CA2AABA3972E843C6119C332E1", "F463E5D03B7BC74F0EA1E92DEE6EB9738A1D6AA706B5B901CE79F913A0F182D50E53BC818A94831B7D07FEE7AA3248218955E3393A809A1F1898FDB0B7B2D059" }
                });

            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "Id", "BaselinePrice", "DateCreated", "DateUpdated", "Description", "EndTime", "ForceClosedBy", "ForceClosedReason", "IsDeleted", "Name", "SellerId", "StartTime", "Status" },
                values: new object[,]
                {
                    { new Guid("19d17521-1036-418b-9911-613e607c7be4"), 300m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A beautiful painting of a city at night, perfect for art lovers.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "City Night Painting", new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { new Guid("4f32e981-8c1e-4e3c-8e47-c98122d8ed49"), 850m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A powerful workstation laptop, ideal for professionals and gamers.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "Workstation Laptop", new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { new Guid("9619c8d2-52ce-4185-b5c4-06a304ae936b"), 250m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sneakers from a high-end fashion brand, perfect for collectors.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "High End Fashion Sneakers", new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 },
                    { new Guid("a507ebae-effd-4317-aa59-8df75282a953"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A well-maintained Mercedes C-Class from 2010, perfect for city driving and long trips.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "Mercedes C-Class 2010", new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 1 }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "Id", "AssignedBy", "AssignedByName", "DateCreated", "DateUpdated", "IsDeleted", "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("ac2a6499-f569-4dae-9eea-2fea12859abf"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("22222222-2222-2222-2222-222222222222"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("997ac6b9-75bc-4889-8cff-656ab45e954a"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("33333333-3333-3333-3333-333333333333"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("d99f646b-3e0f-49ca-a1a5-752464939f0a"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("1667e160-695c-4be8-b7a0-5e2a20de381f"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("55555555-5555-5555-5555-555555555555"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("f0f9bf98-4b47-453d-8ef4-c51bbe644b56"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("66666666-6666-6666-6666-666666666666"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("9409c52e-685e-4554-acb1-d77b91571a6f"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("77777777-7777-7777-7777-777777777777"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("beabb24d-8666-4b11-9b40-59c6024fc7e4"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("88888888-8888-8888-8888-888888888888"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("939c7ba5-03c2-4779-8ce0-9fca2ab45375"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("99999999-9999-9999-9999-999999999999"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("07bd98b0-0c87-4926-b001-5bee0b5fcb1c"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") },
                    { new Guid("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("63291f30-83ab-4786-9469-f003ffadb39d"), new Guid("2193d4c2-3d86-4059-a642-e5338c51167a") }
                });

            migrationBuilder.InsertData(
                table: "UserRoles",
                columns: new[] { "Id", "AssignedBy", "AssignedByName", "DateCreated", "DateUpdated", "IsDeleted", "RoleId", "UserId" },
                values: new object[] { new Guid("55555555-5555-5555-5555-555555555555"), null, "system", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("2193d4c2-3d86-4059-a642-e5338c51167a"), new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e") });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "DateCreated", "DateUpdated", "IsDeleted", "UserId" },
                values: new object[,]
                {
                    { new Guid("14e2427a-99f2-47d5-a02d-e565e212fc03"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa") },
                    { new Guid("1da693bc-9c40-4ca4-a0f4-1c5af1a9d391"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992") },
                    { new Guid("aa9bf01e-3879-4ce7-8ebb-07a18818ebe7"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("783855e1-d39d-402a-9235-175eaf1eb472") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuctionImages_AuctionId",
                table: "AuctionImages",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_AuctionImages_IsDeleted",
                table: "AuctionImages",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_IsDeleted",
                table: "Auctions",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_SellerId",
                table: "Auctions",
                column: "SellerId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationTokens_IsDeleted",
                table: "AuthenticationTokens",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationTokens_UserId",
                table: "AuthenticationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_AuctionId",
                table: "Bids",
                column: "AuctionId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_BidderId",
                table: "Bids",
                column: "BidderId");

            migrationBuilder.CreateIndex(
                name: "IX_Bids_IsDeleted",
                table: "Bids",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_IsDeleted",
                table: "Permissions",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_IsDeleted",
                table: "RolePermissions",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_RoleId",
                table: "RolePermissions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_IsDeleted",
                table: "Roles",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_IsDeleted",
                table: "UserRoles",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_RoleId",
                table: "UserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_IsDeleted",
                table: "Users",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTokens_IsDeleted",
                table: "VerificationTokens",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_VerificationTokens_UserId",
                table: "VerificationTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_IsDeleted",
                table: "Wallets",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_Wallets_UserId",
                table: "Wallets",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_BidId",
                table: "WalletTransactions",
                column: "BidId");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_IsDeleted",
                table: "WalletTransactions",
                column: "IsDeleted",
                filter: "\"IsDeleted\" = false");

            migrationBuilder.CreateIndex(
                name: "IX_WalletTransactions_WalletId",
                table: "WalletTransactions",
                column: "WalletId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuctionImages");

            migrationBuilder.DropTable(
                name: "AuthenticationTokens");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "VerificationTokens");

            migrationBuilder.DropTable(
                name: "WalletTransactions");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Bids");

            migrationBuilder.DropTable(
                name: "Wallets");

            migrationBuilder.DropTable(
                name: "Auctions");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
