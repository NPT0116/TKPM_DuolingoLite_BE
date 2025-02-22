using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCOnfigureOfOptionAndWord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Words_Medias_ImageId",
                table: "Words");

            migrationBuilder.DropTable(
                name: "UserActivity");

            migrationBuilder.DropTable(
                name: "UserProfile");

            migrationBuilder.DropIndex(
                name: "IX_Words_ImageId",
                table: "Words");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Words");

            migrationBuilder.AlterColumn<string>(
                name: "VietnameseText",
                table: "Options",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateTable(
                name: "UserActivities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivities_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: true),
                    NickName = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    ProfileImageId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    userId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfiles_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfiles_Medias_ProfileImageId",
                        column: x => x.ProfileImageId,
                        principalTable: "Medias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfiles_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserActivities_userId",
                table: "UserActivities",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_ProfileImageId",
                table: "UserProfiles",
                column: "ProfileImageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_SubscriptionId",
                table: "UserProfiles",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_userId",
                table: "UserProfiles",
                column: "userId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserActivities");

            migrationBuilder.DropTable(
                name: "UserProfiles");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageId",
                table: "Words",
                type: "uuid",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "VietnameseText",
                table: "Options",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "UserActivity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserActivity_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserProfile",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProfileImageId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubscriptionId = table.Column<Guid>(type: "uuid", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    NickName = table.Column<string>(type: "text", nullable: false),
                    userId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProfile", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserProfile_AspNetUsers_userId",
                        column: x => x.userId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Medias_ProfileImageId",
                        column: x => x.ProfileImageId,
                        principalTable: "Medias",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserProfile_Subscriptions_SubscriptionId",
                        column: x => x.SubscriptionId,
                        principalTable: "Subscriptions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Words_ImageId",
                table: "Words",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserActivity_userId",
                table: "UserActivity",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_ProfileImageId",
                table: "UserProfile",
                column: "ProfileImageId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_SubscriptionId",
                table: "UserProfile",
                column: "SubscriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProfile_userId",
                table: "UserProfile",
                column: "userId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Words_Medias_ImageId",
                table: "Words",
                column: "ImageId",
                principalTable: "Medias",
                principalColumn: "Id");
        }
    }
}
