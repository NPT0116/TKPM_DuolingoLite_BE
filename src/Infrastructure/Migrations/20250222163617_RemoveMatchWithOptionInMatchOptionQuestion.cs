using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveMatchWithOptionInMatchOptionQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionOptions_Options_MatchWithOptionId",
                table: "QuestionOptions");

            migrationBuilder.DropIndex(
                name: "IX_QuestionOptions_MatchWithOptionId",
                table: "QuestionOptions");

            migrationBuilder.DropColumn(
                name: "MatchWithOptionId",
                table: "QuestionOptions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "MatchWithOptionId",
                table: "QuestionOptions",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOptions_MatchWithOptionId",
                table: "QuestionOptions",
                column: "MatchWithOptionId");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionOptions_Options_MatchWithOptionId",
                table: "QuestionOptions",
                column: "MatchWithOptionId",
                principalTable: "Options",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
