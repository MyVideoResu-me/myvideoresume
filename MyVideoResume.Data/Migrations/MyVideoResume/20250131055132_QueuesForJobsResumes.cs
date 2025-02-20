using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class QueuesForJobsResumes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "QueueJobToResumeEntityId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "QueueResumeToJobEntityId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "QueueJobToResume",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueJobToResume", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueJobToResume_Jobs_JobId",
                        column: x => x.JobId,
                        principalTable: "Jobs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QueueResumeToJob",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ResumeItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    StartBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndBatchProcessDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreationDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateDateTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedDateTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QueueResumeToJob", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QueueResumeToJob_ResumeInformation_ResumeItemId",
                        column: x => x.ResumeItemId,
                        principalTable: "ResumeInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantToJob_QueueJobToResumeEntityId",
                table: "ApplicantToJob",
                column: "QueueJobToResumeEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicantToJob_QueueResumeToJobEntityId",
                table: "ApplicantToJob",
                column: "QueueResumeToJobEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueJobToResume_JobId",
                table: "QueueJobToResume",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "IX_QueueResumeToJob_ResumeItemId",
                table: "QueueResumeToJob",
                column: "ResumeItemId");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_QueueJobToResume_QueueJobToResumeEntityId",
                table: "ApplicantToJob",
                column: "QueueJobToResumeEntityId",
                principalTable: "QueueJobToResume",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_QueueResumeToJob_QueueResumeToJobEntityId",
                table: "ApplicantToJob",
                column: "QueueResumeToJobEntityId",
                principalTable: "QueueResumeToJob",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_QueueJobToResume_QueueJobToResumeEntityId",
                table: "ApplicantToJob");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_QueueResumeToJob_QueueResumeToJobEntityId",
                table: "ApplicantToJob");

            migrationBuilder.DropTable(
                name: "QueueJobToResume");

            migrationBuilder.DropTable(
                name: "QueueResumeToJob");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantToJob_QueueJobToResumeEntityId",
                table: "ApplicantToJob");

            migrationBuilder.DropIndex(
                name: "IX_ApplicantToJob_QueueResumeToJobEntityId",
                table: "ApplicantToJob");

            migrationBuilder.DropColumn(
                name: "QueueJobToResumeEntityId",
                table: "ApplicantToJob");

            migrationBuilder.DropColumn(
                name: "QueueResumeToJobEntityId",
                table: "ApplicantToJob");
        }
    }
}
