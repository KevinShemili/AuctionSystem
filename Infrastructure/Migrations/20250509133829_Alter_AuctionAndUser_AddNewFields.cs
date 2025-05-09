using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Alter_AuctionAndUser_AddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BlockReason",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ForceClosedBy",
                table: "Auctions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ForceClosedReason",
                table: "Auctions",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "BlockReason", "PasswordHash", "PasswordSalt" },
                values: new object[] { null, "E9D28C04BB18622A3B4AF098D998E09D19C3921DC0622F6D9D102B585C9C1CEF392B31541B91A83783E1A276C4DBC8A3794203F95A22D637C8780FB0BD8F02F0", "8B6307059EED0CFBDE9203D33BD28BB072574CF1162F1A945437270D3016D28E6739516A2356C9EAD4D239883668F3089CF9CFB8D12780C581736519FD28C7A2" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlockReason",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ForceClosedBy",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "ForceClosedReason",
                table: "Auctions");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "F83429B86AF0059DBB626337CD3FA7C4FA87F3734931AC8292B9031F805D01C741FB8804A7940E0A9C2E2755E392BA0D93F2A4CD7DD3BCDB08A713D5667CE654", "271B883728EDAD2E7554A726028193B2C32C91AB44177583D7A22040F95D0AF0A365DEB5854D80CB1153C8A22F30144CF1F8975A71AACCBD2FA09819E629B568" });
        }
    }
}
