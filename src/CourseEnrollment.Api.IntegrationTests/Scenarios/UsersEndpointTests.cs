using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Integrations.Extensions;
using System.Linq;

namespace CourseEnrollment.Api.Integrations.Scenarios
{
    public class UsersEndointTests : TestBase
    {
        private UserDto UserDtoReq { get; } = new UserDto { Id = Guid.Empty, Email = $"john.wayne@wayland.corp" };

        public UsersEndointTests(WebApplicationFactory<Startup> fixture)
         : base(fixture, "UserEndpointTests")
        {
        }

        [Fact]
        public async Task GetByUserId_WhenUserNotFound_ReturnNotFound()
        {
            var getResponse = await Client.GetAsync($"{UsersEndpoint}/{Guid.NewGuid()}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateUser_WhenUserIsCreated_ItCanBeFetched()
        {
            var userDtoResp = await CreateUser();
            userDtoResp.Id.Should().NotBe(Guid.Empty);
            userDtoResp.Email.Should().Be(UserDtoReq.Email);

            var getResponse = await Client.GetAsync($"{UsersEndpoint}/{userDtoResp.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var fetchedUserDto = await getResponse.ToUserDto();
            fetchedUserDto.Should().Be(userDtoResp);
        }

        [Fact]
        public async Task CreateUser_WhenUserExists_ReturnsUnprocessableEntity()
        {
            await Client.PostAsync($"{UsersEndpoint}", UserDtoReq.ToStringContent());

            var postResponse = await Client.PostAsync($"{UsersEndpoint}", UserDtoReq.ToStringContent());
            postResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task DeleteUser_WhenDeletingNotExistentUser_ReturnNotFound()
        {
            var deleteResponse = await Client.DeleteAsync($"{UsersEndpoint}/{UserDtoReq.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteUser_UserExistsAndAfterDeletetionDissapears()
        {
            var userDtoResp = await CreateUser();

            var deleteResponse = await Client.DeleteAsync($"{UsersEndpoint}/{userDtoResp.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync($"{UsersEndpoint}/{userDtoResp.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task EnrollUser_UserDoesNotExists_ReturnBadRequest()
        {
            var enrollUserReq = new List<EnrollWithdrawUserDto> { new EnrollWithdrawUserDto() { Id = Guid.NewGuid() } };

            var postResponse = await Client.PostAsync(
                $"{UsersEndpoint}/{Guid.NewGuid()}/relationship/courses",
                enrollUserReq.ToStringContent());

            postResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task EnrollUser_Successful_FetchCourseReturnEnrolledCourses()
        {
            var course1Dto = await CreateCourse("Algerbra");
            var course2Dto = await CreateCourse("Computer Science");

            var user = await CreateUser();

            await EnrollUser(user, course1Dto, course2Dto);

            var listOfUserCourses = await GetEnrolledCourses(user);

            listOfUserCourses.Count.Should().Be(2);
            listOfUserCourses.Any(c => c == course1Dto).Should().BeTrue();
            listOfUserCourses.Any(c => c == course2Dto).Should().BeTrue();

            await Withdraw(user, course1Dto, course2Dto);

            var listOfUserCoursesAfterWithdraw = await GetEnrolledCourses(user);
            listOfUserCoursesAfterWithdraw.Count.Should().Be(0);

        }

        private async Task<CourseDto> CreateCourse(string courseName)
        {
            var courseDtoReq = new CourseDto() { Name = courseName };
            var postResponse = await Client.PostAsync($"{CoursesEndpoint}", courseDtoReq.ToStringContent());
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            return await postResponse.ToCourseDto();
        }

        private async Task<UserDto> CreateUser()
        {
            var postResponse = await Client.PostAsync($"{UsersEndpoint}", UserDtoReq.ToStringContent());
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var userDtoResp = await postResponse.ToUserDto();
            return userDtoResp;
        }

        private async Task EnrollUser(UserDto user, CourseDto course1Dto, CourseDto course2Dto)
        {
            var enrollUserReq = new List<EnrollWithdrawUserDto>
            {
                new EnrollWithdrawUserDto() { Id = course1Dto.Id },
                new EnrollWithdrawUserDto() { Id = course2Dto.Id }
            };
            var enrollCourseResponse = await Client.PostAsync(
                    $"{UsersEndpoint}/{user.Id}/relationship/courses", enrollUserReq.ToStringContent());
            enrollCourseResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        private async Task<IList<CourseDto>> GetEnrolledCourses(UserDto user)
        {
            var getEnrolledCoursesResponse = await Client.GetAsync(
                    $"{UsersEndpoint}/{user.Id}/relationship/courses");
            getEnrolledCoursesResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            return await getEnrolledCoursesResponse.ToCourseListDto();
        }

        private async Task Withdraw(UserDto user, CourseDto course1Dto, CourseDto course2Dto)
        {
            var enrollUserReq = new List<EnrollWithdrawUserDto>
            {
                new EnrollWithdrawUserDto() { Id = course1Dto.Id },
                new EnrollWithdrawUserDto() { Id = course2Dto.Id }
            };
            var enrollCourseResponse = await Client.DeleteWithBodyAsync(
                    $"{UsersEndpoint}/{user.Id}/relationship/courses", enrollUserReq.ToStringContent());
            enrollCourseResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
    }
}