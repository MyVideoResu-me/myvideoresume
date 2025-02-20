using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class MYV18WatchStructureUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchedResumes_UserProfiles_UserId",
                table: "WatchedResumes");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "WatchedResumes",
                newName: "UserProfileId");

            migrationBuilder.RenameIndex(
                name: "IX_WatchedResumes_UserId",
                table: "WatchedResumes",
                newName: "IX_WatchedResumes_UserProfileId");

            migrationBuilder.AddColumn<bool>(
                name: "IsWatched",
                table: "ResumeInformation",
                type: "bit",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_WatchedResumes_UserProfiles_UserProfileId",
                table: "WatchedResumes",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WatchedResumes_UserProfiles_UserProfileId",
                table: "WatchedResumes");

            migrationBuilder.DropColumn(
                name: "IsWatched",
                table: "ResumeInformation");

            migrationBuilder.RenameColumn(
                name: "UserProfileId",
                table: "WatchedResumes",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_WatchedResumes_UserProfileId",
                table: "WatchedResumes",
                newName: "IX_WatchedResumes_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WatchedResumes_UserProfiles_UserId",
                table: "WatchedResumes",
                column: "UserId",
                principalTable: "UserProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
