using System.Net.Http;
using System;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace CourseEnrollment.Api.Integrations.Scenarios
{
    public class TestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        protected const string UsersEndpoint = "users";
        protected const string CoursesEndpoint = "courses";
        protected const string ServiceUrl = "https://course-enrollment-service.northpass.com/";
        protected HttpClient Client { get; }

        public TestBase(WebApplicationFactory<Startup> fixture, string fixtureName)
        {
            Client = fixture.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(fixtureName);
            }).CreateClient();
            Client.BaseAddress = new Uri(ServiceUrl);
        }
    }
}