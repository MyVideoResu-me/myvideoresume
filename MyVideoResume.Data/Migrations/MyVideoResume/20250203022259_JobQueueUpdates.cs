using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class JobQueueUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "LastProcessingStatus",
                table: "JobWebsites",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "QueueJobToJobEntityId",
                table: "Jobs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QueueJobToJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueJobToJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueJobToJob_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Jobs_QueueJobToJobEntityId",
                table: "Jobs",
                column: "QueueJobToJobEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueJobToJob_JobId",
                table: "QueueJobToJob",
                column: "JobId");

            migrationBuilder.AddForeignKey(
                name: "FK_Jobs_QueueJobToJob_QueueJobToJobEntityId",
                table: "Jobs",
                column: "QueueJobToJobEntityId",
                principalTable: "QueueJobToJob",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Jobs_QueueJobToJob_QueueJobToJobEntityId",
                table: "Jobs");

            migrationBuilder.DropTable(
                name: "QueueJobToJob");

            migrationBuilder.DropIndex(
                name: "IX_Jobs_QueueJobToJobEntityId",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "QueueJobToJobEntityId",
                table: "Jobs");

            migrationBuilder.AlterColumn<string>(
                name: "LastProcessingStatus",
                table: "JobWebsites",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
