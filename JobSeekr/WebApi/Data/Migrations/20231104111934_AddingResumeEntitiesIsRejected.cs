using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddingResumeEntitiesIsRejected : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsRejected",
                table: "Resumes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "HashPassword", "IsBlocked", "Role" },
                values: new object[] { 228L, "admin@gmail.com", "$2a$11$IwUi8LOTPtv2DZOFIOX.BO7T9SL7QFcP4h8Zj22eA6z.aNCMZC6Hi", false, 2 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 228L);

            migrationBuilder.DropColumn(
                name: "IsRejected",
                table: "Resumes");
        }
    }
}
