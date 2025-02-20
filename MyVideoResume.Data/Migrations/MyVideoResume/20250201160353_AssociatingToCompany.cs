using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class AssociatingToCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserCompanyRoles");

            migrationBuilder.AddColumn<Guid>(
                name: "CompanyProfileEntityId",
                table: "UserProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserCompanyRolesAssociation",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    InviteStatus = table.Column<int>(type: "int", nullable: false),
                    InviteStatusStartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InviteStatusEndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RolesAssigned = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompanyRolesAssociation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompanyRolesAssociation_Companies_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserCompanyRolesAssociation_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserProfiles_CompanyProfileEntityId",
                table: "UserProfiles",
                column: "CompanyProfileEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyRolesAssociation_CompanyProfileId",
                table: "UserCompanyRolesAssociation",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyRolesAssociation_UserProfileId",
                table: "UserCompanyRolesAssociation",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProfiles_Companies_CompanyProfileEntityId",
                table: "UserProfiles",
                column: "CompanyProfileEntityId",
                principalTable: "Companies",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProfiles_Companies_CompanyProfileEntityId",
                table: "UserProfiles");

            migrationBuilder.DropTable(
                name: "UserCompanyRolesAssociation");

            migrationBuilder.DropIndex(
                name: "IX_UserProfiles_CompanyProfileEntityId",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CompanyProfileEntityId",
                table: "UserProfiles");

            migrationBuilder.CreateTable(
                name: "UserCompanyRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InviteStatus = table.Column<int>(type: "int", nullable: false),
                    InviteStatusEndDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    InviteStatusStartDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RolesAssigned = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserCompanyRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserCompanyRoles_Companies_CompanyProfileId",
                        column: x => x.CompanyProfileId,
                        principalTable: "Companies",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserCompanyRoles_UserProfiles_UserProfileId",
                        column: x => x.UserProfileId,
                        principalTable: "UserProfiles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyRoles_CompanyProfileId",
                table: "UserCompanyRoles",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyRoles_UserProfileId",
                table: "UserCompanyRoles",
                column: "UserProfileId");
        }
    }
}
