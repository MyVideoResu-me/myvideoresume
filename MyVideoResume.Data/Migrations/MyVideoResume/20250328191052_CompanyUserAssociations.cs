using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class CompanyUserAssociations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserProfiles_AssignedToUserId1",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserProfiles_CreatedByUserId1",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Companies_CompanyProfileEntityId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_CompanyProfileEntityId",
                table: "UserProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToUserId1",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedByUserId1",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CompanyProfileEntityId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AssignedToUserId1",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId1",
                table: "Tasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "CreatedByUserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "AssignedToUserId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedByUserId",
                table: "Tasks",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserProfiles_AssignedToUserId",
                table: "Tasks",
                column: "AssignedToUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserProfiles_CreatedByUserId",
                table: "Tasks",
                column: "CreatedByUserId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserProfiles_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_UserProfiles_CreatedByUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_AssignedToUserId",
                table: "Tasks");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_CreatedByUserId",
                table: "Tasks");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyProfileEntityId",
                table: "UserProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreatedByUserId",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "AssignedToUserId",
                table: "Tasks",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<Guid>(
                name: "AssignedToUserId1",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CreatedByUserId1",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CompanyProfileEntityId",
                table: "UserProfiles",
                column: "CompanyProfileEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId1",
                table: "Tasks",
                column: "AssignedToUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedByUserId1",
                table: "Tasks",
                column: "CreatedByUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserProfiles_AssignedToUserId1",
                table: "Tasks",
                column: "AssignedToUserId1",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_UserProfiles_CreatedByUserId1",
                table: "Tasks",
                column: "CreatedByUserId1",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Companies_CompanyProfileEntityId",
                table: "UserProfiles",
                column: "CompanyProfileEntityId",
                principalTable: "Companies",
                principalColumn: "Id");
        }
    }
}
