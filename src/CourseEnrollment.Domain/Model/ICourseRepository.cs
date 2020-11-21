using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CourseEnrollment.Domain.Model
{
    public interface ICourseRepository
    {
        Task<Course> AddAsync(Course course);

        IList<Course> GetAll();

        Task<Course> GetByCourseIdAsync(Guid courseId);

        Task DeleteAsync(Course course);

        Task<bool> CourseExistsAsync(string courseName);
    }
}