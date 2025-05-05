using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens");

            migrationBuilder.RenameTable(
                name: "RefreshTokens",
                newName: "AuthenticationTokens");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_UserId",
                table: "AuthenticationTokens",
                newName: "IX_AuthenticationTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_RefreshTokens_IsDeleted",
                table: "AuthenticationTokens",
                newName: "IX_AuthenticationTokens_IsDeleted");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuthenticationTokens",
                table: "AuthenticationTokens",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "5A7F8905C754E3A6D71F1B47EA34AA9070A8609CA17D6C3CCFC66E21E32012A3A205280FA1EACBB87457197EEB8B37F72C8D68E9C3DE93DA2B92EC59B32F55AF", "C12062A48E7BC42709B4B964D67C82FA02CC4460D1DBA0F8B791649FE2C4D0CCB867FFFA51E2C8954D1D7D6A0ADDEE412D369F268F1CA1E9ACC5BD63364C6FBF" });

            migrationBuilder.AddForeignKey(
                name: "FK_AuthenticationTokens_Users_UserId",
                table: "AuthenticationTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuthenticationTokens_Users_UserId",
                table: "AuthenticationTokens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuthenticationTokens",
                table: "AuthenticationTokens");

            migrationBuilder.RenameTable(
                name: "AuthenticationTokens",
                newName: "RefreshTokens");

            migrationBuilder.RenameIndex(
                name: "IX_AuthenticationTokens_UserId",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AuthenticationTokens_IsDeleted",
                table: "RefreshTokens",
                newName: "IX_RefreshTokens_IsDeleted");

            migrationBuilder.AddPrimaryKey(
                name: "PK_RefreshTokens",
                table: "RefreshTokens",
                column: "Id");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "4ACC92FAF0D85C4B1D7D238F1AD224066D94643283BF70BFE3C901FAF13696D3968E39298DE8D9D316BB1E58DCEAAA18970AC66A9D87B7881F28874BDBA1324C", "1C61786FB7F2EFB83CE14A1E3D0CB85A1B2F2A431653982278ABF3C3DB0B7002A174BD96CB7400FBD8C276AC81A4BBE0293B6CB5C39D43B911737F79D0CB37C2" });

            migrationBuilder.AddForeignKey(
                name: "FK_RefreshTokens_Users_UserId",
                table: "RefreshTokens",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
