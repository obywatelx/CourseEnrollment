using System.Threading.Tasks;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json;
using CourseEnrollment.Api.DTOs;

namespace CourseEnrollment.Api.Integrations.Extensions
{
    public static class HttpResponseMessageExtension
    {
        public static async Task<UserDto> ToUserDto(this HttpResponseMessage httpResponseMessage)
        {
            var jsonContent = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserDto>(jsonContent);
        }

        public static async Task<CourseDto> ToCourseDto(this HttpResponseMessage httpResponseMessage)
        {
            var jsonContent = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<CourseDto>(jsonContent);
        }

        public static async Task<IList<CourseDto>> ToCourseListDto(this HttpResponseMessage httpResponseMessage)
        {
            var jsonContent = await httpResponseMessage.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<List<CourseDto>>(jsonContent);
        }
    }
}