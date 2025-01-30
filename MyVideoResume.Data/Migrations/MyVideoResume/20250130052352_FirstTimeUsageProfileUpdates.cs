using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class FirstTimeUsageProfileUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "UserProfiles");

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountUsageType",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsPaidAccountDateTime",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidPurchaseDateTime",
                table: "UserProfiles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PaidPurchasePrice",
                table: "UserProfiles",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AccountUsageType",
                table: "Companies",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "IsPaidAccountDateTime",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "PaidPurchaseDateTime",
                table: "Companies",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "PaidPurchasePrice",
                table: "Companies",
                type: "float",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AccountUsageType",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "IsPaidAccountDateTime",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PaidPurchaseDateTime",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "PaidPurchasePrice",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "AccountType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "AccountUsageType",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "IsPaidAccountDateTime",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PaidPurchaseDateTime",
                table: "Companies");

            migrationBuilder.DropColumn(
                name: "PaidPurchasePrice",
                table: "Companies");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
