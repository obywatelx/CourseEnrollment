using System;
using System.ComponentModel.DataAnnotations;
using Swashbuckle.AspNetCore.Annotations;

namespace CourseEnrollment.Api.DTOs
{
    public record UserDto
    {
        [SwaggerSchema("The user identifier", ReadOnly = true)]
        public Guid Id { get; set; }

        [Required]
        [EmailAddress(ErrorMessage = "Not a valid email")]
        public string Email { get; set; }
    }
}