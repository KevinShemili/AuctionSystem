using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Some_Seeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                values: new object[] { "62C635156BC512A9B6106B694F0C4C05B2685817840EC47FC720EAA117B6B811B3C4821E27ADC2AE476FE14EB35C301C434B3AA939C23CD72CF3E53BC5EB1F2D", "509BF1AFB8A5AB60FFC4ED8758E7682F2B2B86CBC07D7D0D931C8ADDA8222A0CA32489A30D1A97FEC3E8EB52738F15D556ED14C1CA1CEF6003B23CAB64CFEA13" });
        }
    }
}
