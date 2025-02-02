using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class TasksActivities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Addresses_MailingAddressId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "Companies");

            migrationBuilder.RenameColumn(
                name: "MailingAddressId",
                table: "UserProfiles",
                newName: "BoardEntityId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_MailingAddressId",
                table: "UserProfiles",
                newName: "IX_UserProfiles_BoardEntityId");

            migrationBuilder.AddColumn<string>(
                name: "SocialProfiles",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "SocialProfiles",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "Intake",
                table: "ApplicantToJob",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "JobCampaignId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RejectionStatus",
                table: "ApplicantToJob",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ContactType",
                table: "Addresses",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsDefault",
                table: "Addresses",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileEntityId",
                table: "Addresses",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Activity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MyProperty = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApplicantToJobEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Activity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Activity_ApplicantToJob_ApplicantToJobEntityId",
                        column: x => x.ApplicantToJobEntityId,
                        principalTable: "ApplicantToJob",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Boards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Boards_Companies_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Boards_UserProfiles_CreatedByUserId1",
                        column: x => x.CreatedByUserId1,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignedToUserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedByUserId1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CompanyProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApplicantToJobEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BoardEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedByUserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AssignedToUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TaskType = table.Column<int>(type: "int", nullable: false),
                    SubTaskType = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_ApplicantToJob_ApplicantToJobEntityId",
                        column: x => x.ApplicantToJobEntityId,
                        principalTable: "ApplicantToJob",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Boards_BoardEntityId",
                        column: x => x.BoardEntityId,
                        principalTable: "Boards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_Companies_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_UserProfiles_AssignedToUserId1",
                        column: x => x.AssignedToUserId1,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tasks_UserProfiles_CreatedByUserId1",
                        column: x => x.CreatedByUserId1,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Addresses_UserProfileEntityId",
                table: "Addresses",
                column: "UserProfileEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Activity_ApplicantToJobEntityId",
                table: "Activity",
                column: "ApplicantToJobEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_CompanyProfileId",
                table: "Boards",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Boards_CreatedByUserId1",
                table: "Boards",
                column: "CreatedByUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ApplicantToJobEntityId",
                table: "Tasks",
                column: "ApplicantToJobEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_AssignedToUserId1",
                table: "Tasks",
                column: "AssignedToUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_BoardEntityId",
                table: "Tasks",
                column: "BoardEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CompanyProfileId",
                table: "Tasks",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_CreatedByUserId1",
                table: "Tasks",
                column: "CreatedByUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Addresses_UserProfiles_UserProfileEntityId",
                table: "Addresses",
                column: "UserProfileEntityId",
                principalTable: "UserProfiles",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Boards_BoardEntityId",
                table: "UserProfiles",
                column: "BoardEntityId",
                principalTable: "Boards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Addresses_UserProfiles_UserProfileEntityId",
                table: "Addresses");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Boards_BoardEntityId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "Activity");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Boards");

            migrationBuilder.DropIndex(
                name: "IX_Addresses_UserProfileEntityId",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "SocialProfiles",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "SocialProfiles",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "Intake",
                table: "ApplicantToJob");

            migrationBuilder.DropColumn(
                name: "JobCampaignId",
                table: "ApplicantToJob");

            migrationBuilder.DropColumn(
                name: "RejectionStatus",
                table: "ApplicantToJob");

            migrationBuilder.DropColumn(
                name: "ContactType",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "IsDefault",
                table: "Addresses");

            migrationBuilder.DropColumn(
                name: "UserProfileEntityId",
                table: "Addresses");

            migrationBuilder.RenameColumn(
                name: "BoardEntityId",
                table: "UserProfiles",
                newName: "MailingAddressId");

            migrationBuilder.RenameIndex(
                name: "IX_UserProfiles_BoardEntityId",
                table: "UserProfiles",
                newName: "IX_UserProfiles_MailingAddressId");

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Addresses_MailingAddressId",
                table: "UserProfiles",
                column: "MailingAddressId",
                principalTable: "Addresses",
                principalColumn: "Id");
        }
    }
}
