using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class JobProcessingQueue : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SocialProfiles",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "QueueResumeToResumeEntityId",
                table: "ResumeInformation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SocialProfiles",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateTable(
                name: "JobWebsites",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ParsingRegularExpression = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    LastProcessingStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobWebsites", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QueueResumeToResume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueResumeToResume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueResumeToResume_ResumeInformation_ResumeItemId",
                        column: x => x.ResumeItemId,
                        principalTable: "ResumeInformation",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "QueueJobToProcessEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobProcessedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    WebsiteId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Attempts = table.Column<int>(type: "int", nullable: false),
                    AttemptLastDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FailedAttemptMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueJobToProcessEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueJobToProcessEntity_JobWebsites_WebsiteId",
                        column: x => x.WebsiteId,
                        principalTable: "JobWebsites",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_QueueJobToProcessEntity_Jobs_JobProcessedId",
                        column: x => x.JobProcessedId,
                        principalTable: "Jobs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_ResumeInformation_QueueResumeToResumeEntityId",
                table: "ResumeInformation",
                column: "QueueResumeToResumeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueJobToProcessEntity_JobProcessedId",
                table: "QueueJobToProcessEntity",
                column: "JobProcessedId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueJobToProcessEntity_WebsiteId",
                table: "QueueJobToProcessEntity",
                column: "WebsiteId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueResumeToResume_ResumeItemId",
                table: "QueueResumeToResume",
                column: "ResumeItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ResumeInformation_QueueResumeToResume_QueueResumeToResumeEntityId",
                table: "ResumeInformation",
                column: "QueueResumeToResumeEntityId",
                principalTable: "QueueResumeToResume",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ResumeInformation_QueueResumeToResume_QueueResumeToResumeEntityId",
                table: "ResumeInformation");

            migrationBuilder.DropTable(
                name: "QueueJobToProcessEntity");

            migrationBuilder.DropTable(
                name: "QueueResumeToResume");

            migrationBuilder.DropTable(
                name: "JobWebsites");

            migrationBuilder.DropIndex(
                name: "IX_ResumeInformation_QueueResumeToResumeEntityId",
                table: "ResumeInformation");

            migrationBuilder.DropColumn(
                name: "QueueResumeToResumeEntityId",
                table: "ResumeInformation");

            migrationBuilder.AlterColumn<string>(
                name: "SocialProfiles",
                table: "UserProfiles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SocialProfiles",
                table: "Companies",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
