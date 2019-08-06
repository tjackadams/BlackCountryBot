using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace BlackCountryBot.Core.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateSequence(
                name: "EntityFrameworkHiLoSequence",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "Phrases",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false),
                    Original = table.Column<string>(maxLength: 100, nullable: false),
                    Translation = table.Column<string>(maxLength: 100, nullable: false),
                    LastTweetTime = table.Column<DateTimeOffset>(nullable: true),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: false),
                    NumberOfTweets = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Phrases", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Phrases");

            migrationBuilder.DropSequence(
                name: "EntityFrameworkHiLoSequence");
        }
    }
}
