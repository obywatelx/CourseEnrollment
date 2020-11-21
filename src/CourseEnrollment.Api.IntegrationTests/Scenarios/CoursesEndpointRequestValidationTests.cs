using System;
using System.Net;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Integrations.Extensions;

namespace CourseEnrollment.Api.Integrations.Scenarios
{
    public class CoursesEndointRequestValidationTests : TestBase
    {
        public CoursesEndointRequestValidationTests(WebApplicationFactory<Startup> fixture)
            : base(fixture, "CourseEndpointRequestValidationTests")
        {

        }

        [Fact]
        public async Task PostCourse_IdPresent_ReturnBadRequest()
        {
            var courseDto = new CourseDto() { Id = Guid.NewGuid(), Name = "Mathematics" };
            var postResponse = await Client.PostAsync($"{UsersEndpoint}", courseDto.ToStringContent());

            postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostCourse_EnrolledPresent_ReturnBadRequest()
        {
            var courseDto = new CourseDto() { Name = "Mathematics", Enrolled = 10 };
            var postResponse = await Client.PostAsync($"{UsersEndpoint}", courseDto.ToStringContent());

            postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostCourse_MissingName_ReturnBadRequest()
        {
            var courseDto = new CourseDto() { };
            var postResponse = await Client.PostAsync($"{UsersEndpoint}", courseDto.ToStringContent());

            postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}