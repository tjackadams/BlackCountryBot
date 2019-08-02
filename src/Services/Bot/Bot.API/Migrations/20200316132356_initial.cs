using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Bot.API.Migrations
{
    public partial class initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "bot");

            migrationBuilder.CreateSequence(
                name: "translationseq",
                schema: "bot",
                incrementBy: 10);

            migrationBuilder.CreateTable(
                name: "requests",
                schema: "bot",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "translations",
                schema: "bot",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SequenceHiLo),
                    CreatedTime = table.Column<DateTimeOffset>(nullable: false),
                    LastTweetTime = table.Column<DateTimeOffset>(nullable: true),
                    TweetCount = table.Column<int>(nullable: false),
                    OriginalPhrase = table.Column<string>(maxLength: 140, nullable: false),
                    TranslatedPhrase = table.Column<string>(maxLength: 140, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_translations", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "requests",
                schema: "bot");

            migrationBuilder.DropTable(
                name: "translations",
                schema: "bot");

            migrationBuilder.DropSequence(
                name: "translationseq",
                schema: "bot");
        }
    }
}
