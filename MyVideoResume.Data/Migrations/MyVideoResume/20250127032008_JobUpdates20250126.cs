using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class JobUpdates20250126 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidAccount",
                table: "UserProfiles",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermsOfUseAgreementAcceptedDateTime",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfUserAgreementVersion",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Privacy",
                table: "MetaData",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContactUserId",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContactUserId1",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidAccount",
                table: "Companies",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_ContactUserId1",
                table: "Jobs",
                column: "ContactUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_CreatedByUserId",
                table: "Jobs",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_UserProfiles_ContactUserId1",
                table: "Jobs",
                column: "ContactUserId1",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_UserProfiles_CreatedByUserId",
                table: "Jobs",
                column: "CreatedByUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_UserProfiles_ContactUserId1",
                table: "Jobs");

            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_UserProfiles_CreatedByUserId",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_ContactUserId1",
                table: "Jobs");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_CreatedByUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsPaidAccount",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "TermsOfUseAgreementAcceptedDateTime",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "TermsOfUserAgreementVersion",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Privacy",
                table: "MetaData");

            migrationBuilder.DropColumn(
                name: "ContactUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "ContactUserId1",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "IsPaidAccount",
                table: "Companies");
        }
    }
}
