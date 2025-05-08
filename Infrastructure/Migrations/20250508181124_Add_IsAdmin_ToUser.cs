using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_IsAdmin_ToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAdministrator",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "IsAdministrator", "PasswordHash", "PasswordSalt" },
                values: new object[] { false, "F83429B86AF0059DBB626337CD3FA7C4FA87F3734931AC8292B9031F805D01C741FB8804A7940E0A9C2E2755E392BA0D93F2A4CD7DD3BCDB08A713D5667CE654", "271B883728EDAD2E7554A726028193B2C32C91AB44177583D7A22040F95D0AF0A365DEB5854D80CB1153C8A22F30144CF1F8975A71AACCBD2FA09819E629B568" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAdministrator",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "F90DCAB61FA7B1BC9D578C48B90B22D6F336996F251F1F55EA30A07EDA6203FDF58B6E389BDB341E2263FDF33055BFEF9A2BEB4171BFE2C0663AA1C2CC5A856B", "665468D47305115EB4C3A8D35DA420AE0897C44BEBEE1BD2990CF6CEB0812838A4B0237A80C4C5E31531A92128FE7C04E390A1BB3C0EAE300534A6E8229D6452" });
        }
    }
}
