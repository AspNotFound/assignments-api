using Assignment.Infrastructure.Ef.Entities;
using Microsoft.EntityFrameworkCore;

namespace Assignment.Infrastructure.Ef
{
    public static class Queries
    {
        public static IQueryable<AssignmentEntity> AssignmentQuery(this IQueryable<AssignmentEntity> query)
        {
            return query;
        }

        public static IQueryable<SubmissionEntity> SubmissionQuery(this IQueryable<SubmissionEntity> query)
        {
            return query
                .Include(s => s.Attachments)
                .Include(s => s.Judgement)
                    .ThenInclude(s => s!.GradingSystemGrade);
        }

        public static IQueryable<GradingSystemEntity> GradingSystemQuery(this IQueryable<GradingSystemEntity> query)
        {
            return query.Include(gs => gs.Grades);
        }
    }
}