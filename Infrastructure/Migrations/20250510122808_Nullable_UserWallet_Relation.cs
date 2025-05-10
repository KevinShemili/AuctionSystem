using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Nullable_UserWallet_Relation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "4A16B1F552C23880A665FE0636C0D35FBBD29E2B36155C227AA5F58B996A992D7BC165FE1722C674C57137AA06C0B8912E0E62190C03A50614851224A91522D7", "6F1FD8D42AD19E35B523B94ED60B7401C3CE15C8A5F1686C2C7ACBAA3DD4AF9B23E913ABE15D4BF9EC11E70188743FC52C3C4B3BEF7FE0F6DBFD1E8F0A9C21C2" });

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "CFE7ECDA6446E08AB0D53B05A80CA0C8345C3116EBFC979C4BBB8506906231BCE541E5A58A3A2DEFA02DA8A6FDCAD9DBC788E59642025011CD22C12BCF570ACA", "2A922A1E3E3AE6619430EDBA4CF6E5DED96CB3CBE4C01FD55F4DF05B5C3929E9CFD552FB90D148D8277AEA0169AEB0B9C1D5483C46BD06F0FB033649989635FD" });

            migrationBuilder.AddForeignKey(
                name: "FK_Wallets_Users_UserId",
                table: "Wallets",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
