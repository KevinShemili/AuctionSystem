using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Change_AuctionImageFormat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Data",
                table: "AuctionImages");

            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "AuctionImages",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "F90DCAB61FA7B1BC9D578C48B90B22D6F336996F251F1F55EA30A07EDA6203FDF58B6E389BDB341E2263FDF33055BFEF9A2BEB4171BFE2C0663AA1C2CC5A856B", "665468D47305115EB4C3A8D35DA420AE0897C44BEBEE1BD2990CF6CEB0812838A4B0237A80C4C5E31531A92128FE7C04E390A1BB3C0EAE300534A6E8229D6452" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "AuctionImages");

            migrationBuilder.AddColumn<byte[]>(
                name: "Data",
                table: "AuctionImages",
                type: "bytea",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "820287AAA7C71F25A4E09EC1258C8D68895B9EF64666C3453F63B0951A887283D13B1583EBE1B456EBEB95C44B37F525C2662593A8F960634D2013B22E58CA4B", "E19D25DF29F01BEDD7E7FCDEBAB79E2A32C9A2A17801DEBFA78704A090E45AD4BF766BE86EBEC04E2A74815641C4E0C2FCE1802360CDB84F5861EDA887FB32F3" });
        }
    }
}
