using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Auction_Seeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Auctions",
                columns: new[] { "Id", "BaselinePrice", "DateCreated", "DateUpdated", "Description", "EndTime", "ForceClosedBy", "ForceClosedReason", "IsDeleted", "Name", "SellerId", "StartTime", "Status" },
                values: new object[,]
                {
                    { new Guid("19d17521-1036-418b-9911-613e607c7be4"), 300m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A beautiful painting of a city at night, perfect for art lovers.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "City Night Painting", new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { new Guid("4f32e981-8c1e-4e3c-8e47-c98122d8ed49"), 850m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A powerful workstation laptop, ideal for professionals and gamers.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "Workstation Laptop", new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { new Guid("9619c8d2-52ce-4185-b5c4-06a304ae936b"), 250m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "Sneakers from a high-end fashion brand, perfect for collectors.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "High End Fashion Sneakers", new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0 },
                    { new Guid("a507ebae-effd-4317-aa59-8df75282a953"), 1000m, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "A well-maintained Mercedes C-Class from 2010, perfect for city driving and long trips.", new DateTime(2025, 1, 3, 0, 0, 0, 0, DateTimeKind.Utc), null, null, false, "Mercedes C-Class 2010", new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), 0 }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "B6A47A6149DD7EA70F2DD58484DAAB210EDA9FFB64EBA70D9B9B62BAB00243FC51D4CBFE30DCDE5C8B3D54D20E7A6C3B4C52AC98CC20BE9391D7533AF7921644", "17EDD51F0D912E62D8320FA1324045F519B8AFA64F1446F68DEAE300A174A1980BE1FABDF4B502E1089DF4C84A49B8FAF906BDFBBC2C2DF7BEA6697F7CA6E7D1" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("783855e1-d39d-402a-9235-175eaf1eb472"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "B6A47A6149DD7EA70F2DD58484DAAB210EDA9FFB64EBA70D9B9B62BAB00243FC51D4CBFE30DCDE5C8B3D54D20E7A6C3B4C52AC98CC20BE9391D7533AF7921644", "17EDD51F0D912E62D8320FA1324045F519B8AFA64F1446F68DEAE300A174A1980BE1FABDF4B502E1089DF4C84A49B8FAF906BDFBBC2C2DF7BEA6697F7CA6E7D1" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "B6A47A6149DD7EA70F2DD58484DAAB210EDA9FFB64EBA70D9B9B62BAB00243FC51D4CBFE30DCDE5C8B3D54D20E7A6C3B4C52AC98CC20BE9391D7533AF7921644", "17EDD51F0D912E62D8320FA1324045F519B8AFA64F1446F68DEAE300A174A1980BE1FABDF4B502E1089DF4C84A49B8FAF906BDFBBC2C2DF7BEA6697F7CA6E7D1" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "B6A47A6149DD7EA70F2DD58484DAAB210EDA9FFB64EBA70D9B9B62BAB00243FC51D4CBFE30DCDE5C8B3D54D20E7A6C3B4C52AC98CC20BE9391D7533AF7921644", "17EDD51F0D912E62D8320FA1324045F519B8AFA64F1446F68DEAE300A174A1980BE1FABDF4B502E1089DF4C84A49B8FAF906BDFBBC2C2DF7BEA6697F7CA6E7D1" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("19d17521-1036-418b-9911-613e607c7be4"));

            migrationBuilder.DeleteData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("4f32e981-8c1e-4e3c-8e47-c98122d8ed49"));

            migrationBuilder.DeleteData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("9619c8d2-52ce-4185-b5c4-06a304ae936b"));

            migrationBuilder.DeleteData(
                table: "Auctions",
                keyColumn: "Id",
                keyValue: new Guid("a507ebae-effd-4317-aa59-8df75282a953"));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("5fc111de-788d-49b2-ba73-7a1e62e9c42e"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("783855e1-d39d-402a-9235-175eaf1eb472"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("8884546c-45cc-496e-97b1-b7c861c3cafa"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("c75ce5c0-cf73-44be-849b-7e1de26ae992"),
                columns: new[] { "PasswordHash", "PasswordSalt" },
                values: new object[] { "4BB93947F95E96BC11D6C3571B9BD5EBA2C8265D3326433369A260505F385FD68C17140ECA6F023C9D04892593F3535C11D7F28989E315CBB2795D42DD102584", "80FA97ED8D320FD157D360274FFB940FE3BBECC994300D856A366B422F28665F7610D8C4D9E9A9422D8D16F6A436D4EBE55FC69F6D586DD0BB0BEDCE68D4425F" });
        }
    }
}
