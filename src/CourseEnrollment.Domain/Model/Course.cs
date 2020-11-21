using System;
using System.Collections.Generic;

namespace CourseEnrollment.Domain.Model
{
    public record Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public IList<User> Users { get; }

        public Course(Guid id, string name)
        {
            Id = id;
            Name = name;
            Users = new List<User>();
        }
    }
}