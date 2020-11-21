using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using Newtonsoft.Json;
using CourseEnrollment.Api.DTOs;

namespace CourseEnrollment.Api.Integrations.Extensions
{
    public static class DtoExtensions
    {
        public static StringContent ToStringContent(this UserDto userDto)
        {
            string json = JsonConvert.SerializeObject(userDto);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static StringContent ToStringContent(this CourseDto courseDto)
        {
            string json = JsonConvert.SerializeObject(courseDto);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }

        public static StringContent ToStringContent(this IList<EnrollWithdrawUserDto> courseDto)
        {
            string json = JsonConvert.SerializeObject(courseDto);
            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }
}