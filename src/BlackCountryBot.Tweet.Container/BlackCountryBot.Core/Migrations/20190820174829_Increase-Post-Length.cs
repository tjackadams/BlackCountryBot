using Microsoft.EntityFrameworkCore.Migrations;

namespace BlackCountryBot.Core.Migrations
{
    public partial class IncreasePostLength : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Translation",
                table: "Phrases",
                maxLength: 140,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Original",
                table: "Phrases",
                maxLength: 140,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 100);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Translation",
                table: "Phrases",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 140);

            migrationBuilder.AlterColumn<string>(
                name: "Original",
                table: "Phrases",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 140);
        }
    }
}
