using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using CourseEnrollment.Infrastructure;

namespace CourseEnrollment.Api.Integrations
{
    public static class WebHostBuilderExtension
    {
        public static void ConfigureTestServices(this IWebHostBuilder builder, string databaseName)
        {
            builder.ConfigureServices(services =>
              {
                  var descriptor = services.SingleOrDefault(
                      d => d.ServiceType ==
                          typeof(DbContextOptions<CourseEnrollmentContext>));

                  services.Remove(descriptor);

                  services.AddDbContext<CourseEnrollmentContext>(options =>
                  {
                      options.UseInMemoryDatabase(databaseName);
                  });

                  var serviceProvider = services.BuildServiceProvider();
                  using (var scope = serviceProvider.CreateScope())
                  {
                      var scopedServices = scope.ServiceProvider;
                      var db = scopedServices
                          .GetRequiredService<CourseEnrollmentContext>();
                      db.Users.RemoveRange(db.Users);
                      db.Courses.RemoveRange(db.Courses);
                      db.SaveChanges();
                  }
              });
        }
    }
}