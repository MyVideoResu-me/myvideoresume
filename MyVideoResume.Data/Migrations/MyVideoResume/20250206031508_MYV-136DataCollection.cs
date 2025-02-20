using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class MYV136DataCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProfileStatus",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProfileStatusDataTime",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Salary",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Salary",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDateTime",
                table: "Salary",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Equity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Equity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDateTime",
                table: "Equity",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDateTime",
                table: "Bonus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDateTime",
                table: "Bonus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdateDateTime",
                table: "Bonus",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DataCollectionRequestLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserAgent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Referrer = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DataCollectionType = table.Column<int>(type: "int", nullable: true),
                    Browser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BrowserVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OSVersion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserProfileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyProfileId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataCollectionRequestLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DataCollectionRequestLogs");

            migrationBuilder.DropColumn(
                name: "ProfileStatus",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "ProfileStatusDataTime",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Salary");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Salary");

            migrationBuilder.DropColumn(
                name: "UpdateDateTime",
                table: "Salary");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Equity");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Equity");

            migrationBuilder.DropColumn(
                name: "UpdateDateTime",
                table: "Equity");

            migrationBuilder.DropColumn(
                name: "CreationDateTime",
                table: "Bonus");

            migrationBuilder.DropColumn(
                name: "DeletedDateTime",
                table: "Bonus");

            migrationBuilder.DropColumn(
                name: "UpdateDateTime",
                table: "Bonus");
        }
    }
}
