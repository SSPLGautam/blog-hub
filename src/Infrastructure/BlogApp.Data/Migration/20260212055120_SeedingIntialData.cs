using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BlogApp.Migrations
{
    /// <inheritdoc />
    public partial class SeedingIntialData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1, null, "Technology" },
                    { 2, null, "Health" },
                    { 3, null, "LifeStyle" }
                });

            migrationBuilder.InsertData(
                table: "Posts",
                columns: new[] { "Id", "Authore", "CategoryId", "Content", "FeatureImagePath", "PublishedDate", "Title" },
                values: new object[,]
                {
                    { 1, "Yash Yadav", 2, "Content of Health Post 1", "~/Image/Health1.jpg", new DateTime(2023, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Health Post 1" },
                    { 2, "Ravi ", 1, "Content of Teach Post 1", "~/Image/Teach.jpg", new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Teach Post 1" },
                    { 3, "joi Davel ", 3, "Content of LifeStyle Post 1", "~/Image/download.jpg", new DateTime(2025, 12, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "LifeStyle Post 1" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Categories",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
