using Assignment.Infrastructure.Ef.Contexts;
using Assignment.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef;

public static class ApplyExtensions
{
    public static void Apply(this AssignmentEntity target, AssignmentEntity source, AssignmentContext context)
    {
        context.Entry(target).CurrentValues.SetValues(source);
    }

    public static void Apply(this SubmissionEntity target, SubmissionEntity source, AssignmentContext context)
    {
        context.Entry(target).CurrentValues.SetValues(source);

        var attachmentsToDelete = target.Attachments.Where(a => source.Attachments.All(sa => sa.Id != a.Id)).ToList();
        foreach (var attachment in attachmentsToDelete)
        {
            context.Entry(attachment).State = EntityState.Deleted;
            target.Attachments.Remove(attachment);
        }

        foreach (var sourceAttachment in source.Attachments)
        {
            var targetAttachment = target.Attachments.FirstOrDefault(a => a.Id == sourceAttachment.Id);
            if (targetAttachment == null)
            {
                context.Entry(sourceAttachment).State = EntityState.Added;
                target.Attachments.Add(sourceAttachment);
            }
            else
            {
                targetAttachment.Apply(sourceAttachment, context);
            }
        }

        if (source.Judgement == null && target.Judgement != null)
        {
            context.Entry(target.Judgement).State = EntityState.Deleted;
            target.Judgement = null;
            return;
        }

        if (source.Judgement != null)
        {
            if (target.Judgement == null)
            {
                context.Entry(source.Judgement).State = EntityState.Added;
                target.Judgement = source.Judgement;
            }
            else
            {
                target.Judgement.Apply(source.Judgement, context);
            }
        }
    }

    public static void Apply(this SubmissionAttachmentEntity target, SubmissionAttachmentEntity source, AssignmentContext context)
    {
        context.Entry(target).CurrentValues.SetValues(source);
    }

    public static void Apply(this SubmissionJudgementEntity target, SubmissionJudgementEntity source, AssignmentContext context)
    {
        context.Entry(target).CurrentValues.SetValues(source);
    }

    public static void Apply(this GradingSystemEntity target, GradingSystemEntity source, AssignmentContext context)
    {
        context.Entry(target).CurrentValues.SetValues(source);

        var gradesToDelete = target.Grades.Where(g => source.Grades.All(sg => sg.Id != g.Id)).ToList();
        foreach (var grade in gradesToDelete)
        {
            context.Entry(grade).State = EntityState.Deleted;
            target.Grades.Remove(grade);
        }

        foreach (var sourceGrade in source.Grades)
        {
            var targetGrade = target.Grades.FirstOrDefault(g => g.Id == sourceGrade.Id);
            if (targetGrade == null)
            {
                context.Entry(sourceGrade).State = EntityState.Added;
                target.Grades.Add(sourceGrade);
            }
            else
            {
                targetGrade.Apply(sourceGrade, context);
            }
        }
    }

    public static void Apply(this GradingSystemGradeEntity target, GradingSystemGradeEntity source, AssignmentContext context)
    {
        context.Entry(target).CurrentValues.SetValues(source);
    }
}