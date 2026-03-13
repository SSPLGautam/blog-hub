using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlogApp.Migrations
{
    /// <inheritdoc />
    public partial class ColumnAdded_Posts_CreatedByUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "Posts",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedByUserId",
                value: "df194139-131d-4147-bda8-4bb661e3eec3");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedByUserId",
                value: "df194139-131d-4147-bda8-4bb661e3eec3");

            migrationBuilder.UpdateData(
                table: "Posts",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedByUserId",
                value: "df194139-131d-4147-bda8-4bb661e3eec3");

            migrationBuilder.CreateIndex(
                name: "IX_Posts_CreatedByUserId",
                table: "Posts",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Posts_AspNetUsers_CreatedByUserId",
                table: "Posts",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Posts_AspNetUsers_CreatedByUserId",
                table: "Posts");

            migrationBuilder.DropIndex(
                name: "IX_Posts_CreatedByUserId",
                table: "Posts");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Posts");
        }
    }
}
