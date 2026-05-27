using Assignment.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef.Contexts;

public class AssignmentContext(DbContextOptions<AssignmentContext> options) : DbContext(options)
{
    public DbSet<AssignmentEntity> Assignments { get; set; } = null!;
    public DbSet<GradingSystemEntity> GradingSystems { get; set; } = null!;
    public DbSet<GradingSystemGradeEntity> GradingSystemGrades { get; set; } = null!;
    public DbSet<SubmissionEntity> Submissions { get; set; } = null!;
    public DbSet<SubmissionAttachmentEntity> SubmissionAttachments { get; set; } = null!;
    public DbSet<SubmissionJudgementEntity> SubmissionJudgements { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("assignment");
        modelBuilder.Entity<AssignmentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CourseId).IsRequired();
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.Description).IsRequired();
            entity.Property(e => e.Deadline).IsRequired();
            entity.HasOne(e => e.GradingSystem)
                .WithMany(gs => gs.Assignments)
                .HasForeignKey(e => e.GradingSystemId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<GradingSystemEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();

            entity.HasMany(e => e.Grades)
                .WithOne(g => g.GradingSystem)
                .HasForeignKey(g => g.GradingSystemId)
                .OnDelete(DeleteBehavior.NoAction);
        });

        modelBuilder.Entity<GradingSystemGradeEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.IsPassingGrade).IsRequired();
            entity.Property(e => e.Order).IsRequired();

            entity.HasOne(e => e.GradingSystem)
                .WithMany(gs => gs.Grades)
                .HasForeignKey(e => e.GradingSystemId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => new { e.GradingSystemId, e.Order }).IsUnique();
            entity.HasIndex(e => new { e.GradingSystemId, e.Name }).IsUnique();
        });

        modelBuilder.Entity<SubmissionAttachmentEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.FileName).IsRequired();
            entity.Property(e => e.FileUrl).IsRequired();

            entity.HasOne(e => e.Submission)
                .WithMany(s => s.Attachments)
                .HasForeignKey(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => e.Name).IsUnique();
        });

        modelBuilder.Entity<SubmissionEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AuthorId).IsRequired();
            entity.Property(e => e.Content).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ModifiedAt).IsRequired();
            entity.Property(e => e.AssignmentId).IsRequired();
            entity.Property(e => e.JudgementId).IsRequired(false);

            entity.HasOne(e => e.Assignment)
                .WithMany(a => a.Submissions)
                .HasForeignKey(e => e.AssignmentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Judgement)
                .WithOne(j => j.Submission)
                .HasForeignKey<SubmissionJudgementEntity>(j => j.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.AssignmentId, e.AuthorId }).IsUnique();
        });

        modelBuilder.Entity<SubmissionJudgementEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.JudgeId).IsRequired();
            entity.Property(e => e.Feedback).IsRequired();
            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.ModifiedAt).IsRequired();

            entity.HasOne(e => e.Submission)
                .WithOne(s => s.Judgement)
                .HasForeignKey<SubmissionJudgementEntity>(e => e.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.GradingSystemGrade)
                .WithMany(grade => grade.SubmissionJudgements)
                .HasForeignKey(e => e.GradingSystemGradeId)
                .OnDelete(DeleteBehavior.NoAction);

            entity.HasIndex(e => e.SubmissionId).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }

}