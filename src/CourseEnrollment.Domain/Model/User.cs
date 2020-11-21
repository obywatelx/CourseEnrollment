using System;
using System.Collections.Generic;

namespace CourseEnrollment.Domain.Model
{
    public record User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public IList<Course> Courses { get; }

        public User(Guid id, string email)
        {
            Id = id;
            Email = email;
            Courses = new List<Course>();
        }

        public void Enroll(Course course)
        {
            Courses.Add(course);
        }

        public void Withdrawn(Course course)
        {
            Courses.Remove(course);
        }
    }
}