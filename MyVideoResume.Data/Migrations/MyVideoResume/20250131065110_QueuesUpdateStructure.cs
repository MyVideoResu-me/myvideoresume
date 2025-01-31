using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class QueuesUpdateStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_Jobs_JobId",
                table: "ApplicantToJob");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_ResumeInformation_ResumeItemId",
                table: "ApplicantToJob");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_UserProfiles_UserApplyingId",
                table: "ApplicantToJob");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantToJob_JobId",
                table: "ApplicantToJob");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantToJob_ResumeItemId",
                table: "ApplicantToJob");

            migrationBuilder.RenameColumn(
                name: "UserApplyingId",
                table: "ApplicantToJob",
                newName: "UserProfileEntityId");

            migrationBuilder.RenameColumn(
                name: "ResumeItemId",
                table: "ApplicantToJob",
                newName: "UserProfileEntityApplyingId");

            migrationBuilder.RenameColumn(
                name: "JobId",
                table: "ApplicantToJob",
                newName: "ResumeInformationEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantToJob_UserApplyingId",
                table: "ApplicantToJob",
                newName: "IX_ApplicantToJob_UserProfileEntityId");

            migrationBuilder.AddColumn<Guid>(
                name: "JobItemEntityId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_UserProfiles_UserProfileEntityId",
                table: "ApplicantToJob",
                column: "UserProfileEntityId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_UserProfiles_UserProfileEntityId",
                table: "ApplicantToJob");

            migrationBuilder.DropColumn(
                name: "JobItemEntityId",
                table: "ApplicantToJob");

            migrationBuilder.RenameColumn(
                name: "UserProfileEntityId",
                table: "ApplicantToJob",
                newName: "UserApplyingId");

            migrationBuilder.RenameColumn(
                name: "UserProfileEntityApplyingId",
                table: "ApplicantToJob",
                newName: "ResumeItemId");

            migrationBuilder.RenameColumn(
                name: "ResumeInformationEntityId",
                table: "ApplicantToJob",
                newName: "JobId");

            migrationBuilder.RenameIndex(
                name: "IX_ApplicantToJob_UserProfileEntityId",
                table: "ApplicantToJob",
                newName: "IX_ApplicantToJob_UserApplyingId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantToJob_JobId",
                table: "ApplicantToJob",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantToJob_ResumeItemId",
                table: "ApplicantToJob",
                column: "ResumeItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_Jobs_JobId",
                table: "ApplicantToJob",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_ResumeInformation_ResumeItemId",
                table: "ApplicantToJob",
                column: "ResumeItemId",
                principalTable: "ResumeInformation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_UserProfiles_UserApplyingId",
                table: "ApplicantToJob",
                column: "UserApplyingId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }
    }
}
