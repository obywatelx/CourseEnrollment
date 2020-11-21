using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseEnrollment.Api.DTOs
{
    public record CourseDto
    {
        [SwaggerSchema("The course identifier", ReadOnly = true)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }

        [SwaggerSchema("The number of users enrolled in course", ReadOnly = true)]
        public int Enrolled { get; set; }
    }
}