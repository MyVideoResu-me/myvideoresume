using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class ProductivityUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AccountUsageType",
                table: "UserCompanyRolesAssociation",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidAccount",
                table: "UserCompanyRolesAssociation",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsPaidAccountDateTime",
                table: "UserCompanyRolesAssociation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidPurchaseDateTime",
                table: "UserCompanyRolesAssociation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PaidPurchasePrice",
                table: "UserCompanyRolesAssociation",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "TermsOfUseAgreementAcceptedDateTime",
                table: "UserCompanyRolesAssociation",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TermsOfUserAgreementVersion",
                table: "UserCompanyRolesAssociation",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ProjectEntityId",
                table: "Tasks",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Appointments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LocationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    ActionToTake = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Appointments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Appointments_Addresses_LocationId",
                        column: x => x.LocationId,
                        principalTable: "Addresses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_Boards_BoardEntityId",
                        column: x => x.BoardEntityId,
                        principalTable: "Boards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_Companies_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Appointments_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    ActionToTake = table.Column<int>(type: "int", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_Boards_BoardEntityId",
                        column: x => x.BoardEntityId,
                        principalTable: "Boards",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_Companies_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Projects_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectEntityId",
                table: "Tasks",
                column: "ProjectEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BoardEntityId",
                table: "Appointments",
                column: "BoardEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_CompanyProfileId",
                table: "Appointments",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_LocationId",
                table: "Appointments",
                column: "LocationId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UserProfileId",
                table: "Appointments",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_BoardEntityId",
                table: "Projects",
                column: "BoardEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CompanyProfileId",
                table: "Projects",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_UserProfileId",
                table: "Projects",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Projects_ProjectEntityId",
                table: "Tasks",
                column: "ProjectEntityId",
                principalTable: "Projects",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Projects_ProjectEntityId",
                table: "Tasks");

            migrationBuilder.DropTable(
                name: "Appointments");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Tasks_ProjectEntityId",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "AccountUsageType",
                table: "UserCompanyRolesAssociation");

            migrationBuilder.DropColumn(
                name: "IsPaidAccount",
                table: "UserCompanyRolesAssociation");

            migrationBuilder.DropColumn(
                name: "IsPaidAccountDateTime",
                table: "UserCompanyRolesAssociation");

            migrationBuilder.DropColumn(
                name: "PaidPurchaseDateTime",
                table: "UserCompanyRolesAssociation");

            migrationBuilder.DropColumn(
                name: "PaidPurchasePrice",
                table: "UserCompanyRolesAssociation");

            migrationBuilder.DropColumn(
                name: "TermsOfUseAgreementAcceptedDateTime",
                table: "UserCompanyRolesAssociation");

            migrationBuilder.DropColumn(
                name: "TermsOfUserAgreementVersion",
                table: "UserCompanyRolesAssociation");

            migrationBuilder.DropColumn(
                name: "ProjectEntityId",
                table: "Tasks");
        }
    }
}
