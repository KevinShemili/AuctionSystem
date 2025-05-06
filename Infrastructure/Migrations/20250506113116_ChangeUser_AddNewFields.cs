using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeUser_AddNewFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserName",
                table: "Users",
                newName: "LastName");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "FirstName", "PasswordHash", "PasswordSalt" },
                values: new object[] { "admin", "96EB8454320FD2098616A5A9028C997B643573C7C91A94652FA836024919C8FCD968B3AC34E1A5C9CBC6B11C720F9AD79354DFA2461975C4E5E73A788111579E", "DAC28373E0B8A556BE09F635D812F2E2E7FFAAEC6B913FD7DD9E0CE1B504183037BDDB1AE33D61F48ABBAB8E8E922C9BE935AFCBCFBB8249DCD4D015445EB57B" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "LastName",
                table: "Users",
                newName: "UserName");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "934AE1C43916F1F5A8E6E9CD7B81EDA15534ECAF8C220ABD1820F99FAED4A0D75A6704F39EE302544912CD3704D948A9695FDA876E3985F3ECA53B4B1C5E83AD", "6EDE43A2957BADD14E460BB3A518B901186A607F54EC4123C7758DFA86E204EB406ECADC45A8EAAA4CAA081920CE084CB9164FB740E2746EF9DCA4669FEAE6AB" });
        }
    }
}
