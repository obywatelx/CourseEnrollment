using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CourseEnrollment.Domain.Model;


namespace CourseEnrollment.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        public CourseEnrollmentContext Context { get; }

        public CourseRepository(CourseEnrollmentContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<Course> AddAsync(Course course)
        {
            await Context.Courses.AddAsync(course);
            await Context.SaveChangesAsync(CancellationToken.None);
            return course;
        }

        public IList<Course> GetAll()
        {
            return Context.Courses.Include(a => a.Users).ToList();
        }

        public async Task DeleteAsync(Course course)
        {
            var courseToRemove = Context.Courses.Where(u => u.Id == course.Id).First();
            Context.Courses.Remove(courseToRemove);
            await Context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<Course> GetByCourseIdAsync(Guid courseId)
        {
            var course = await Context.Courses.SingleOrDefaultAsync(u => u.Id == courseId);
            if (course == null)
            {
                return null;
            }
            return course;
        }

        public async Task<bool> CourseExistsAsync(string courseName)
        {
            return await Context.Courses.AnyAsync(course => course.Name == courseName);
        }
    }
}