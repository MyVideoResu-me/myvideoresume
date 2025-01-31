using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyVideoResume.Data.Migrations.MyVideoResume
{
    /// <inheritdoc />
    public partial class QueuesForJobsResumesForeignKeyUpdates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_Jobs_JobId",
                table: "ApplicantToJob");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_ResumeInformation_ResumeItemId",
                table: "ApplicantToJob");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueJobToResume_Jobs_JobId",
                table: "QueueJobToResume");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueResumeToJob_ResumeInformation_ResumeItemId",
                table: "QueueResumeToJob");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeItemId",
                table: "QueueResumeToJob",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "QueueJobToResume",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserApplyingId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeItemId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_Jobs_JobId",
                table: "ApplicantToJob",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_ResumeInformation_ResumeItemId",
                table: "ApplicantToJob",
                column: "ResumeItemId",
                principalTable: "ResumeInformation",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueJobToResume_Jobs_JobId",
                table: "QueueJobToResume",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_QueueResumeToJob_ResumeInformation_ResumeItemId",
                table: "QueueResumeToJob",
                column: "ResumeItemId",
                principalTable: "ResumeInformation",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_Jobs_JobId",
                table: "ApplicantToJob");

            migrationBuilder.DropForeignKey(
                name: "FK_ApplicantToJob_ResumeInformation_ResumeItemId",
                table: "ApplicantToJob");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueJobToResume_Jobs_JobId",
                table: "QueueJobToResume");

            migrationBuilder.DropForeignKey(
                name: "FK_QueueResumeToJob_ResumeInformation_ResumeItemId",
                table: "QueueResumeToJob");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeItemId",
                table: "QueueResumeToJob",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "QueueJobToResume",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "UserApplyingId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeItemId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "JobId",
                table: "ApplicantToJob",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_Jobs_JobId",
                table: "ApplicantToJob",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ApplicantToJob_ResumeInformation_ResumeItemId",
                table: "ApplicantToJob",
                column: "ResumeItemId",
                principalTable: "ResumeInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueJobToResume_Jobs_JobId",
                table: "QueueJobToResume",
                column: "JobId",
                principalTable: "Jobs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QueueResumeToJob_ResumeInformation_ResumeItemId",
                table: "QueueResumeToJob",
                column: "ResumeItemId",
                principalTable: "ResumeInformation",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
