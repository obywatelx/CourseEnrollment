using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Integrations.Extensions;
using System.Linq;
using CourseEnrollment.Domain.Model;

namespace CourseEnrollment.Api.Integrations.Scenarios
{
    public class CoursesEndointTests : TestBase
    {
        public CoursesEndointTests(WebApplicationFactory<Startup> fixture)
            : base(fixture, "CourseEndpointTests")
        {
        }

        [Fact]
        public async Task FetchCourse_ReturnListOfCoursesThatExists()
        {
            var course1Dto = await CreateCourse("Physics");
            var course2Dto = await CreateCourse("Mathematics");

            var getResponse = await Client.GetAsync($"{CoursesEndpoint}");

            var courseListDto = await getResponse.ToCourseListDto();
            courseListDto.Count.Should().Be(2);
            courseListDto.Any(c => c == course1Dto).Should().BeTrue();
            courseListDto.Any(c => c == course2Dto).Should().BeTrue();
        }


        [Fact]
        public async Task GetByCourseUserId_WhenCourseNotFound_ReturnNotFound()
        {
            var getResponse = await Client.GetAsync($"{CoursesEndpoint}/{Guid.NewGuid()}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateCourse_WhenCourseIsCreated_ItCanBeFetched()
        {
            const string courseName = "Physics";
            var courseDtoReq = await CreateCourse("Physics");
            courseDtoReq.Id.Should().NotBe(Guid.Empty);
            courseDtoReq.Name.Should().Be(courseName);

            var fetchedCourseDto = await GetCourse(courseDtoReq.Id);
            fetchedCourseDto.Should().Be(courseDtoReq);
        }

        [Fact]
        public async Task CreateCourse_WhenCourseExists_ReturnsUnprocessableEntity()
        {
            var courseDtoReq = await CreateCourse("Physics");

            var secondRequest = courseDtoReq with { Id = Guid.Empty };
            var postResponse = await Client.PostAsync($"{CoursesEndpoint}", secondRequest.ToStringContent());
            postResponse.StatusCode.Should().Be(HttpStatusCode.UnprocessableEntity);
        }

        [Fact]
        public async Task DeleteCourse_WhenDeletingNotExistentCourse_ReturnNotFound()
        {
            var deleteResponse = await Client.DeleteAsync($"{CoursesEndpoint}/{Guid.NewGuid()}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteCourse_CourseExistsAndAfterDeletetionDissapears()
        {
            var courseDtoReq = await CreateCourse("Physics");

            var deleteResponse = await Client.DeleteAsync($"{CoursesEndpoint}/{courseDtoReq.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            var getResponse = await Client.GetAsync($"{CoursesEndpoint}/{courseDtoReq.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        private async Task<CourseDto> CreateCourse(string courseName)
        {
            var courseDtoReq = new CourseDto() { Id = Guid.Empty, Name = courseName };

            var postResponse = await Client.PostAsync($"{CoursesEndpoint}", courseDtoReq.ToStringContent());
            postResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            return await postResponse.ToCourseDto();
        }
        private async Task<CourseDto> GetCourse(Guid courseId)
        {
            var getResponse = await Client.GetAsync($"{CoursesEndpoint}/{courseId}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            return await getResponse.ToCourseDto();
        }
    }
}