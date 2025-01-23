using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class LatLongForJobs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "Addresses",
                newName: "StateProvince");

            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "Addresses",
                newName: "PostalZipCode");

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "ResumeInformation",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "ResumeInformation",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Latitude",
                table: "Jobs",
                type: "float",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Longitude",
                table: "Jobs",
                type: "float",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Longitude",
                table: "Addresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Latitude",
                table: "Addresses",
                type: "float",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "ResumeInformation");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "ResumeInformation");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Jobs");

            migrationBuilder.RenameColumn(
                name: "StateProvince",
                table: "Addresses",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "PostalZipCode",
                table: "Addresses",
                newName: "PostalCode");

            migrationBuilder.AlterColumn<int>(
                name: "Longitude",
                table: "Addresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Latitude",
                table: "Addresses",
                type: "int",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}
