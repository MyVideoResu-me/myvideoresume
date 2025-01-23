using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class CreatingUserToCompanyProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalJobId",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Origin",
                table: "Jobs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "UserProfileId",
                table: "Companies",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Addresses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "UserCompanyRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                name: "IX_Companies_UserProfileId",
                table: "Companies",
                column: "UserProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyRoles_CompanyProfileId",
                table: "UserCompanyRoles",
                column: "CompanyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_UserCompanyRoles_UserProfileId",
                table: "UserCompanyRoles",
                column: "UserProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Companies_UserProfiles_UserProfileId",
                table: "Companies",
                column: "UserProfileId",
                principalTable: "UserProfiles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Companies_UserProfiles_UserProfileId",
                table: "Companies");

            migrationBuilder.DropTable(
                name: "UserCompanyRoles");

            migrationBuilder.DropIndex(
                name: "IX_Companies_UserProfileId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "ExternalJobId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Origin",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UserProfileId",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Addresses");
        }
    }
}
