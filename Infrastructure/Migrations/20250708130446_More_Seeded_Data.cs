using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class More_Seeded_Data : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("e6cec8c2-25f2-4851-adde-4b1a0254f6c8"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("82704478-0262-4116-94ec-ebb174bb6f68"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BlockReason", "DateCreated", "DateUpdated", "Email", "FirstName", "IsDeleted", "IsEmailVerified", "LastName", "PasswordHash", "PasswordSalt" },
                values: new object[,]
                {
                    { new Guid("783855e1-d39d-402a-9235-175eaf1eb472"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevin001sh@gmail.com", "Adam", false, true, "Smith", "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" },
                    { new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevin.shemili@edu.unifi.it", "John", false, true, "Johnson", "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" },
                    { new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevinshemili5@gmail.com", "Kevin", false, true, "Shemili", "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" }
                });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "DateCreated", "DateUpdated", "IsDeleted", "UserId" },
                values: new object[,]
                {
                    { new Guid("14e2427a-99f2-47d5-a02d-e565e212fc03"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa") },
                    { new Guid("1da693bc-9c40-4ca4-a0f4-1c5af1a9d391"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992") },
                    { new Guid("aa9bf01e-3879-4ce7-8ebb-07a18818ebe7"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("783855e1-d39d-402a-9235-175eaf1eb472") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("14e2427a-99f2-47d5-a02d-e565e212fc03"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("1da693bc-9c40-4ca4-a0f4-1c5af1a9d391"));

            migrationBuilder.DeleteData(
                table: "Wallets",
                keyColumn: "Id",
                keyValue: new Guid("aa9bf01e-3879-4ce7-8ebb-07a18818ebe7"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("783855e1-d39d-402a-9235-175eaf1eb472"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"));

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "F1A4073D6510A5D192160B9AC27F61BE7B933762B7FDD9C480F43FD6424025DB6B321A0D28DB913A42AB7FA2AE7DD7326866871A44EDA252ABE96C396B35D506", "F2F59DB8341FA04AA9C0AE248F457C82492D222270EE221AC9B18C3A564C568B934D32FD9FEE8509018FDE0A00683E2E0FBA85B3CC695C4BF51A1D45C17F08FA" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "BlockReason", "DateCreated", "DateUpdated", "Email", "FirstName", "IsDeleted", "IsEmailVerified", "LastName", "PasswordHash", "PasswordSalt" },
                values: new object[] { new Guid("82704478-0262-4116-94ec-ebb174bb6f68"), null, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "kevinshemili5@mail.com", "Kevin", false, true, "Shemili", "F1A4073D6510A5D192160B9AC27F61BE7B933762B7FDD9C480F43FD6424025DB6B321A0D28DB913A42AB7FA2AE7DD7326866871A44EDA252ABE96C396B35D506", "F2F59DB8341FA04AA9C0AE248F457C82492D222270EE221AC9B18C3A564C568B934D32FD9FEE8509018FDE0A00683E2E0FBA85B3CC695C4BF51A1D45C17F08FA" });

            migrationBuilder.InsertData(
                table: "Wallets",
                columns: new[] { "Id", "Balance", "DateCreated", "DateUpdated", "IsDeleted", "UserId" },
                values: new object[] { new Guid("e6cec8c2-25f2-4851-adde-4b1a0254f6c8"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, false, new Guid("82704478-0262-4116-94ec-ebb174bb6f68") });
        }
    }
}
