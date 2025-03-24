using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSpacedRepetition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpacedRepetitionRecords",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionId = table.Column<Guid>(type: "uuid", nullable: false),
                    LastReview = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    NextReview = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    RepetitionCount = table.Column<int>(type: "integer", nullable: false),
                    EasinessFactor = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpacedRepetitionRecords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpacedRepetitionRecords_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpacedRepetitionRecords_QuestionId",
                table: "SpacedRepetitionRecords",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_SpacedRepetitionRecords_UserId_NextReview",
                table: "SpacedRepetitionRecords",
                columns: new[] { "UserId", "NextReview" });

            migrationBuilder.CreateIndex(
                name: "IX_SpacedRepetitionRecords_UserId_QuestionId",
                table: "SpacedRepetitionRecords",
                columns: new[] { "UserId", "QuestionId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpacedRepetitionRecords");
        }
    }
}
