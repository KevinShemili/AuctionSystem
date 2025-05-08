using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Bid_Transaction_OptionalRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Bids_BidId",
                table: "WalletTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "BidId",
                table: "WalletTransactions",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "820287AAA7C71F25A4E09EC1258C8D68895B9EF64666C3453F63B0951A887283D13B1583EBE1B456EBEB95C44B37F525C2662593A8F960634D2013B22E58CA4B", "E19D25DF29F01BEDD7E7FCDEBAB79E2A32C9A2A17801DEBFA78704A090E45AD4BF766BE86EBEC04E2A74815641C4E0C2FCE1802360CDB84F5861EDA887FB32F3" });

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Bids_BidId",
                table: "WalletTransactions",
                column: "BidId",
                principalTable: "Bids",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WalletTransactions_Bids_BidId",
                table: "WalletTransactions");

            migrationBuilder.AlterColumn<Guid>(
                name: "BidId",
                table: "WalletTransactions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "D84D9978471C1A486EC56F6D1CC45F2EC0F7034BC6BB118E58F8E318DAE402E0F2D91622B5DEBBC47910189DD1E1E58FE1920A7D79ED10914B1E7DD06B8F77B3", "7BA19F28E92D1DC5CF6F199D486329BAB94B2D49893D755C2FED387DC4FD9FE94A625271702F4632C2A1F57367BA7BC459828DF75D60DD6CEBFB5E88B992537A" });

            migrationBuilder.AddForeignKey(
                name: "FK_WalletTransactions_Bids_BidId",
                table: "WalletTransactions",
                column: "BidId",
                principalTable: "Bids",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
