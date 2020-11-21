using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CourseEnrollment.Api.Integrations.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseMessage> DeleteWithBodyAsync(
            this HttpClient client, string uri, StringContent stringContent)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Delete,
                RequestUri = new Uri($"https://course-enrollment-service.northpass.com/{uri}"), 
                Content = stringContent
            };
            return await client.SendAsync(request);
        }
    }
}