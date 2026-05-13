using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Modifications : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_GradingSystemEntity_GradingSystemId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_GradingSystemGradeEntity_GradingSystemEntity_GradingSystemId",
                table: "GradingSystemGradeEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionAttachmentEntity_SubmissionEntity_SubmissionId",
                table: "SubmissionAttachmentEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionEntity_Assignments_AssignmentId",
                table: "SubmissionEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionJudgementEntity_GradingSystemGradeEntity_GradingSystemGradeId",
                table: "SubmissionJudgementEntity");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionJudgementEntity_SubmissionEntity_SubmissionId",
                table: "SubmissionJudgementEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionJudgementEntity",
                table: "SubmissionJudgementEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionEntity",
                table: "SubmissionEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionAttachmentEntity",
                table: "SubmissionAttachmentEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GradingSystemGradeEntity",
                table: "GradingSystemGradeEntity");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GradingSystemEntity",
                table: "GradingSystemEntity");

            migrationBuilder.RenameTable(
                name: "SubmissionJudgementEntity",
                newName: "SubmissionJudgements");

            migrationBuilder.RenameTable(
                name: "SubmissionEntity",
                newName: "Submissions");

            migrationBuilder.RenameTable(
                name: "SubmissionAttachmentEntity",
                newName: "SubmissionAttachments");

            migrationBuilder.RenameTable(
                name: "GradingSystemGradeEntity",
                newName: "GradingSystemGrades");

            migrationBuilder.RenameTable(
                name: "GradingSystemEntity",
                newName: "GradingSystems");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionJudgementEntity_SubmissionId",
                table: "SubmissionJudgements",
                newName: "IX_SubmissionJudgements_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionJudgementEntity_GradingSystemGradeId",
                table: "SubmissionJudgements",
                newName: "IX_SubmissionJudgements_GradingSystemGradeId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionEntity_AssignmentId_AuthorId",
                table: "Submissions",
                newName: "IX_Submissions_AssignmentId_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionAttachmentEntity_SubmissionId",
                table: "SubmissionAttachments",
                newName: "IX_SubmissionAttachments_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionAttachmentEntity_Name",
                table: "SubmissionAttachments",
                newName: "IX_SubmissionAttachments_Name");

            migrationBuilder.RenameIndex(
                name: "IX_GradingSystemGradeEntity_GradingSystemId_Order",
                table: "GradingSystemGrades",
                newName: "IX_GradingSystemGrades_GradingSystemId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_GradingSystemGradeEntity_GradingSystemId_Name",
                table: "GradingSystemGrades",
                newName: "IX_GradingSystemGrades_GradingSystemId_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionJudgements",
                table: "SubmissionJudgements",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionAttachments",
                table: "SubmissionAttachments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GradingSystemGrades",
                table: "GradingSystemGrades",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GradingSystems",
                table: "GradingSystems",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_GradingSystems_GradingSystemId",
                table: "Assignments",
                column: "GradingSystemId",
                principalTable: "GradingSystems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GradingSystemGrades_GradingSystems_GradingSystemId",
                table: "GradingSystemGrades",
                column: "GradingSystemId",
                principalTable: "GradingSystems",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionAttachments_Submissions_SubmissionId",
                table: "SubmissionAttachments",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionJudgements_GradingSystemGrades_GradingSystemGradeId",
                table: "SubmissionJudgements",
                column: "GradingSystemGradeId",
                principalTable: "GradingSystemGrades",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionJudgements_Submissions_SubmissionId",
                table: "SubmissionJudgements",
                column: "SubmissionId",
                principalTable: "Submissions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Submissions_Assignments_AssignmentId",
                table: "Submissions",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Assignments_GradingSystems_GradingSystemId",
                table: "Assignments");

            migrationBuilder.DropForeignKey(
                name: "FK_GradingSystemGrades_GradingSystems_GradingSystemId",
                table: "GradingSystemGrades");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionAttachments_Submissions_SubmissionId",
                table: "SubmissionAttachments");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionJudgements_GradingSystemGrades_GradingSystemGradeId",
                table: "SubmissionJudgements");

            migrationBuilder.DropForeignKey(
                name: "FK_SubmissionJudgements_Submissions_SubmissionId",
                table: "SubmissionJudgements");

            migrationBuilder.DropForeignKey(
                name: "FK_Submissions_Assignments_AssignmentId",
                table: "Submissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Submissions",
                table: "Submissions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionJudgements",
                table: "SubmissionJudgements");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubmissionAttachments",
                table: "SubmissionAttachments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GradingSystems",
                table: "GradingSystems");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GradingSystemGrades",
                table: "GradingSystemGrades");

            migrationBuilder.RenameTable(
                name: "Submissions",
                newName: "SubmissionEntity");

            migrationBuilder.RenameTable(
                name: "SubmissionJudgements",
                newName: "SubmissionJudgementEntity");

            migrationBuilder.RenameTable(
                name: "SubmissionAttachments",
                newName: "SubmissionAttachmentEntity");

            migrationBuilder.RenameTable(
                name: "GradingSystems",
                newName: "GradingSystemEntity");

            migrationBuilder.RenameTable(
                name: "GradingSystemGrades",
                newName: "GradingSystemGradeEntity");

            migrationBuilder.RenameIndex(
                name: "IX_Submissions_AssignmentId_AuthorId",
                table: "SubmissionEntity",
                newName: "IX_SubmissionEntity_AssignmentId_AuthorId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionJudgements_SubmissionId",
                table: "SubmissionJudgementEntity",
                newName: "IX_SubmissionJudgementEntity_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionJudgements_GradingSystemGradeId",
                table: "SubmissionJudgementEntity",
                newName: "IX_SubmissionJudgementEntity_GradingSystemGradeId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionAttachments_SubmissionId",
                table: "SubmissionAttachmentEntity",
                newName: "IX_SubmissionAttachmentEntity_SubmissionId");

            migrationBuilder.RenameIndex(
                name: "IX_SubmissionAttachments_Name",
                table: "SubmissionAttachmentEntity",
                newName: "IX_SubmissionAttachmentEntity_Name");

            migrationBuilder.RenameIndex(
                name: "IX_GradingSystemGrades_GradingSystemId_Order",
                table: "GradingSystemGradeEntity",
                newName: "IX_GradingSystemGradeEntity_GradingSystemId_Order");

            migrationBuilder.RenameIndex(
                name: "IX_GradingSystemGrades_GradingSystemId_Name",
                table: "GradingSystemGradeEntity",
                newName: "IX_GradingSystemGradeEntity_GradingSystemId_Name");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionEntity",
                table: "SubmissionEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionJudgementEntity",
                table: "SubmissionJudgementEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubmissionAttachmentEntity",
                table: "SubmissionAttachmentEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GradingSystemEntity",
                table: "GradingSystemEntity",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GradingSystemGradeEntity",
                table: "GradingSystemGradeEntity",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assignments_GradingSystemEntity_GradingSystemId",
                table: "Assignments",
                column: "GradingSystemId",
                principalTable: "GradingSystemEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GradingSystemGradeEntity_GradingSystemEntity_GradingSystemId",
                table: "GradingSystemGradeEntity",
                column: "GradingSystemId",
                principalTable: "GradingSystemEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionAttachmentEntity_SubmissionEntity_SubmissionId",
                table: "SubmissionAttachmentEntity",
                column: "SubmissionId",
                principalTable: "SubmissionEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionEntity_Assignments_AssignmentId",
                table: "SubmissionEntity",
                column: "AssignmentId",
                principalTable: "Assignments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionJudgementEntity_GradingSystemGradeEntity_GradingSystemGradeId",
                table: "SubmissionJudgementEntity",
                column: "GradingSystemGradeId",
                principalTable: "GradingSystemGradeEntity",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SubmissionJudgementEntity_SubmissionEntity_SubmissionId",
                table: "SubmissionJudgementEntity",
                column: "SubmissionId",
                principalTable: "SubmissionEntity",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
