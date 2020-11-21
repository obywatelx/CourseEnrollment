using CourseEnrollment.Api.Application.Commands.CreateUser;
using CourseEnrollment.Domain.Model;
using CourseEnrollment.Api.Application.Commands.CreateCourse;
using CourseEnrollment.Api.DTOs;
using System.Collections;

namespace CourseEnrollment.Api.Extensions
{
    public static class DtoExtensions
    {
        public static UserDto ToUserDto(this User domainUser)
        {
            return new UserDto()
            {
                Id = domainUser.Id,
                Email = domainUser.Email.ToString()
            };
        }

        public static CreateUserCommand ToCreateCommand(this UserDto user)
        {
            return new CreateUserCommand()
            {
                Email = user.Email
            };
        }

        public static CreateCourseCommand ToCreateCommand(this CourseDto user)
        {
            return new CreateCourseCommand()
            {
                Name = user.Name
            };
        }

        public static CourseDto ToCourseDto(this Course courseDto)
        {
            return new CourseDto()
            {
                Id = courseDto.Id,
                Name = courseDto.Name.ToString()
            };
        }
    }
}