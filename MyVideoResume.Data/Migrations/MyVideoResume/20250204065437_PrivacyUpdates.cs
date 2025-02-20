using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class PrivacyUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Privacy_ShowProfile",
                table: "UserProfiles",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Privacy_ShowProfileContactDetails",
                table: "UserProfiles",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Privacy_ShowProfile",
                table: "UserProfiles");

            migrationBuilder.DropColumn(
                name: "Privacy_ShowProfileContactDetails",
                table: "UserProfiles");
        }
    }
}
