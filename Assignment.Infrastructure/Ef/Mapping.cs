using Assignment.Domain.Aggregates;
using Assignment.Domain.Entities;
using Assignment.Infrastructure.Ef.Entities;

namespace Assignment.Infrastructure.Ef
{
    public static class Mapping
    {
        public static Submission ConvertEntityToDomainModel(SubmissionEntity entity)
        {
            return Submission.Hydrate
            (
                entity.Id,
                entity.AssignmentId,
                entity.AuthorId,
                entity.Content,
                [.. entity.Attachments.Select(a => Attachment.Hydrate(a.Id, a.Name, a.FileName, a.FileUrl))],
                entity.CreatedAt,
                entity.ModifiedAt,
                entity.Judgement == null ? null : Judgement.Hydrate
                (
                    entity.Judgement.Id,
                    entity.Judgement.JudgeId,
                    GradingSystemGrade.Hydrate(entity.Judgement.GradingSystemGrade.Id, entity.Judgement.GradingSystemGrade.Name, entity.Judgement.GradingSystemGrade.IsPassingGrade, entity.Judgement.GradingSystemGrade.Order),
                    entity.Judgement.Feedback,
                    entity.Judgement.CreatedAt,
                    entity.Judgement.ModifiedAt
                )
            );

        }

        public static SubmissionEntity ConvertDomainModelToEntity(Submission domainModel)
        {
            return new SubmissionEntity
            {
                Id = domainModel.Id,
                AssignmentId = domainModel.AssignmentId,
                AuthorId = domainModel.AuthorId,
                Content = domainModel.Content,
                CreatedAt = domainModel.CreatedAt,
                ModifiedAt = domainModel.ModifiedAt,
                Attachments = [.. domainModel.Attachments.Select(a => new SubmissionAttachmentEntity
            {
                Id = a.Id,
                Name = a.Name,
                FileName = a.FileName,
                FileUrl = a.Link,
                SubmissionId = domainModel.Id,
            })],
                Judgement = domainModel.Judgement == null ? null : new SubmissionJudgementEntity
                {
                    Id = domainModel.Judgement.Id,
                    CreatedAt = domainModel.Judgement.CreatedAt,
                    GradingSystemGradeId = domainModel.Judgement.Grade.Id,
                    JudgeId = domainModel.Judgement.JudgeId,
                    ModifiedAt = domainModel.Judgement.ModifiedAt,
                    Feedback = domainModel.Judgement.Feedback,
                    SubmissionId = domainModel.Id,
                },
                JudgementId = domainModel.Judgement?.Id
            };

        }

        public static AssignmentEntity ConvertDomainModelToEntity(Domain.Aggregates.Assignment domainModel)
        {
            return new AssignmentEntity
            {
                Id = domainModel.Id,
                GradingSystemId = domainModel.GradingSystemId,
                CourseId = domainModel.CourseId,
                Name = domainModel.Name,
                Description = domainModel.Description,
                Deadline = domainModel.Deadline,
            };
        }

        public static Domain.Aggregates.Assignment ConvertEntityToDomainModel(AssignmentEntity entity)
        {
            return Assignment.Domain.Aggregates.Assignment.Hydrate
            (
                entity.Id,
                entity.GradingSystemId,
                entity.CourseId,
                entity.Name,
                entity.Description,
                entity.Deadline
            );

        }

        public static GradingSystem ConvertEntityToDomainModel(GradingSystemEntity entity)
        {
            return GradingSystem.Hydrate
            (
                entity.Id,
                entity.Name,
                [.. entity.Grades.Select(g => GradingSystemGrade.Hydrate(g.Id, g.Name, g.IsPassingGrade, g.Order))]
            );
        }

        public static GradingSystemEntity ConvertDomainModelToEntity(GradingSystem domainModel)
        {
            return new GradingSystemEntity
            {
                Id = domainModel.Id,
                Name = domainModel.Name,
                Grades = [.. 
                    domainModel.Grades.Select(g => new GradingSystemGradeEntity
                    {
                        Id = g.Id,
                        Name = g.Name,
                        IsPassingGrade = g.IsPassingGrade,
                        GradingSystemId = domainModel.Id,
                        Order = g.Order
                    })
                ]
            };
        }
    }
}