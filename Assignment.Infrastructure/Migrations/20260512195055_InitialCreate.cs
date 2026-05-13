using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GradingSystemEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradingSystemEntity", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assignments",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradingSystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CourseId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Deadline = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assignments_GradingSystemEntity_GradingSystemId",
                        column: x => x.GradingSystemId,
                        principalTable: "GradingSystemEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GradingSystemGradeEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradingSystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPassingGrade = table.Column<bool>(type: "bit", nullable: false),
                    Order = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradingSystemGradeEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradingSystemGradeEntity_GradingSystemEntity_GradingSystemId",
                        column: x => x.GradingSystemId,
                        principalTable: "GradingSystemEntity",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SubmissionEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssignmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    JudgementId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionEntity_Assignments_AssignmentId",
                        column: x => x.AssignmentId,
                        principalTable: "Assignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionAttachmentEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileUrl = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionAttachmentEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionAttachmentEntity_SubmissionEntity_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "SubmissionEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubmissionJudgementEntity",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GradingSystemGradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    JudgeId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Feedback = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubmissionJudgementEntity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubmissionJudgementEntity_GradingSystemGradeEntity_GradingSystemGradeId",
                        column: x => x.GradingSystemGradeId,
                        principalTable: "GradingSystemGradeEntity",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SubmissionJudgementEntity_SubmissionEntity_SubmissionId",
                        column: x => x.SubmissionId,
                        principalTable: "SubmissionEntity",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assignments_GradingSystemId",
                table: "Assignments",
                column: "GradingSystemId");

            migrationBuilder.CreateIndex(
                name: "IX_GradingSystemGradeEntity_GradingSystemId_Name",
                table: "GradingSystemGradeEntity",
                columns: new[] { "GradingSystemId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GradingSystemGradeEntity_GradingSystemId_Order",
                table: "GradingSystemGradeEntity",
                columns: new[] { "GradingSystemId", "Order" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAttachmentEntity_Name",
                table: "SubmissionAttachmentEntity",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionAttachmentEntity_SubmissionId",
                table: "SubmissionAttachmentEntity",
                column: "SubmissionId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionEntity_AssignmentId_AuthorId",
                table: "SubmissionEntity",
                columns: new[] { "AssignmentId", "AuthorId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionJudgementEntity_GradingSystemGradeId",
                table: "SubmissionJudgementEntity",
                column: "GradingSystemGradeId");

            migrationBuilder.CreateIndex(
                name: "IX_SubmissionJudgementEntity_SubmissionId",
                table: "SubmissionJudgementEntity",
                column: "SubmissionId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubmissionAttachmentEntity");

            migrationBuilder.DropTable(
                name: "SubmissionJudgementEntity");

            migrationBuilder.DropTable(
                name: "GradingSystemGradeEntity");

            migrationBuilder.DropTable(
                name: "SubmissionEntity");

            migrationBuilder.DropTable(
                name: "Assignments");

            migrationBuilder.DropTable(
                name: "GradingSystemEntity");
        }
    }
}
