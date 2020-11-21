using System.Net;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using CourseEnrollment.Api.DTOs;
using CourseEnrollment.Api.Integrations.Extensions;
using System;

namespace CourseEnrollment.Api.Integrations.Scenarios
{
    public class UsersEndointRequestValidationTests : TestBase
    {
        public UsersEndointRequestValidationTests(WebApplicationFactory<Startup> fixture)
            : base(fixture, "UserEndpointRequestValidationTests")
        {
        }

        [Fact]
        public async Task PostUser_IdPresent_ReturnBadRequest()
        {
            var userDto = new UserDto() { Id = Guid.NewGuid(), Email = "test@test.pl" };
            var postResponse = await Client.PostAsync($"{UsersEndpoint}", userDto.ToStringContent());

            postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostUser_MissingEmail_ReturnBadRequest()
        {
            var userDto = new UserDto() { };
            var postResponse = await Client.PostAsync($"{UsersEndpoint}", userDto.ToStringContent());

            postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PostUsers_IncorrectEmail_ReturnBadRequest()
        {
            var userDto = new UserDto() { Email = "WronglyFormattedEmail" };
            var postResponse = await Client.PostAsync($"{UsersEndpoint}", userDto.ToStringContent());

            postResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }
    }
}