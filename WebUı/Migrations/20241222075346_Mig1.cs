using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace WebUı.Migrations
{
    /// <inheritdoc />
    public partial class Mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d092a120-2b84-4f56-9885-6e2057ad67a0");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "d2334069-782a-4db2-9d05-2f503125b1a5");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "02976b05-f6a0-4cee-8730-de359f3d477b", "6ac94e8c-5f96-4dcb-976f-18d50bd5185f" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "02976b05-f6a0-4cee-8730-de359f3d477b");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "6ac94e8c-5f96-4dcb-976f-18d50bd5185f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "145a2d63-db57-48f6-baaa-3e588af2a59f", null, "User", "USER" },
                    { "289e6050-faec-4f8a-84b5-91faf11c7c59", null, "Gazetici", "GAZETICI" },
                    { "ad3374aa-2599-4735-bf1c-ce658c7a273c", null, "Admin", "ADMIN" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "d77a79ac-d677-4d79-b6ca-a278d92fb5c9", 0, "190c57c1-0ace-4d7f-a1cf-f02df45842ab", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAIAAYagAAAAEJ7CSTPwvu9Gqo9KKQo6sHEPnlGGWJFh9pBMHkVelFDuOjxHV/KAY7rdsHjiwiJE2A==", null, false, "07bb0c07-bba5-4661-a4dc-1a6882e03680", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "ad3374aa-2599-4735-bf1c-ce658c7a273c", "d77a79ac-d677-4d79-b6ca-a278d92fb5c9" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "145a2d63-db57-48f6-baaa-3e588af2a59f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "289e6050-faec-4f8a-84b5-91faf11c7c59");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "ad3374aa-2599-4735-bf1c-ce658c7a273c", "d77a79ac-d677-4d79-b6ca-a278d92fb5c9" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ad3374aa-2599-4735-bf1c-ce658c7a273c");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "d77a79ac-d677-4d79-b6ca-a278d92fb5c9");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "02976b05-f6a0-4cee-8730-de359f3d477b", null, "Admin", "ADMIN" },
                    { "d092a120-2b84-4f56-9885-6e2057ad67a0", null, "Gazetici", "GAZETICI" },
                    { "d2334069-782a-4db2-9d05-2f503125b1a5", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "6ac94e8c-5f96-4dcb-976f-18d50bd5185f", 0, "89d5d445-648f-4b58-879b-0fbf9f0e0b3c", "admin@admin.com", true, false, null, "ADMIN@ADMIN.COM", "ADMIN", "AQAAAAIAAYagAAAAEGApIowO7txr2iBT+pd9k8+2U6yDEUNzqWXtHOdt/ug5dZ3GW9qEAqbfxwA4pal8aw==", null, false, "e523ee48-34d4-4627-9a9a-8d86dc2cfd55", false, "admin" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "02976b05-f6a0-4cee-8730-de359f3d477b", "6ac94e8c-5f96-4dcb-976f-18d50bd5185f" });
        }
    }
}
