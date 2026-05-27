using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Assignment.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Schema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "assignment");

            migrationBuilder.RenameTable(
                name: "Submissions",
                newName: "Submissions",
                newSchema: "assignment");

            migrationBuilder.RenameTable(
                name: "SubmissionJudgements",
                newName: "SubmissionJudgements",
                newSchema: "assignment");

            migrationBuilder.RenameTable(
                name: "SubmissionAttachments",
                newName: "SubmissionAttachments",
                newSchema: "assignment");

            migrationBuilder.RenameTable(
                name: "GradingSystems",
                newName: "GradingSystems",
                newSchema: "assignment");

            migrationBuilder.RenameTable(
                name: "GradingSystemGrades",
                newName: "GradingSystemGrades",
                newSchema: "assignment");

            migrationBuilder.RenameTable(
                name: "Assignments",
                newName: "Assignments",
                newSchema: "assignment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Submissions",
                schema: "assignment",
                newName: "Submissions");

            migrationBuilder.RenameTable(
                name: "SubmissionJudgements",
                schema: "assignment",
                newName: "SubmissionJudgements");

            migrationBuilder.RenameTable(
                name: "SubmissionAttachments",
                schema: "assignment",
                newName: "SubmissionAttachments");

            migrationBuilder.RenameTable(
                name: "GradingSystems",
                schema: "assignment",
                newName: "GradingSystems");

            migrationBuilder.RenameTable(
                name: "GradingSystemGrades",
                schema: "assignment",
                newName: "GradingSystemGrades");

            migrationBuilder.RenameTable(
                name: "Assignments",
                schema: "assignment",
                newName: "Assignments");
        }
    }
}
