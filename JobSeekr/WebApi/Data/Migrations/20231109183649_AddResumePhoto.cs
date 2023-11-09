using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApi.Migrations
{
    /// <inheritdoc />
    public partial class AddResumePhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResumePhoto",
                columns: table => new
                {
                    ResumeId = table.Column<long>(type: "bigint", nullable: false),
                    Data = table.Column<byte[]>(type: "varbinary(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResumePhoto", x => x.ResumeId);
                    table.ForeignKey(
                        name: "FK_ResumePhoto_Resumes_ResumeId",
                        column: x => x.ResumeId,
                        principalTable: "Resumes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 228L,
                column: "HashPassword",
                value: "$2a$11$yRYiqMnt8346oKKw8vc/lu7UGI3fYNJcwzspPnLu1CsQ2gfqAV5zW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResumePhoto");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 228L,
                column: "HashPassword",
                value: "$2a$11$2rZwEv2RYly1s8m7Gr9zw.24V0jHy52GUkg8wYQT8hBTgqOqkuSQW");
        }
    }
}
