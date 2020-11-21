# Course Enrollment Service
## Running Dockerized Service

To run dockerized Coursse Enrollment service change the dir to src/ and execute following commands:

    docker build -t courseenrollmentservice .

    docker run -d -p 8080:80 --name CourseEnrollment courseenrollmentservice

I enabled Swagger for Release build of the service. It can be accessed from:

    http://localhost:8080/swagger/index.html

Please, not that all operation should use HTTP protocol NOT HTTPS. I disabled HTTPS in the service.

## Runing local build
To run the app locally, without using Docker, go to src/CourseEnrollment.Api and execute
    
    dotnet build

After successfull buil you will need to apply migrations to the database

    dotnet tool install --global dotnet-ef
    dotnet ef database update

## Architecture and Design considerations
I decided I will follow the principles described in [Design a DDD-oriented microservice](https://docs.microsoft.com/en-us/dotnet/architecture/microservices/microservice-ddd-cqrs-patterns/ddd-oriented-microservice). The service described there follows the Hexagonal architecture approach. The application is composed from three components:
* CourseEnrollment.Api - component consisting of WebApi and ApplicationCode
* CourseEnrollment.Domain - domain objects definitions and repositories interfaces
* CourseEnrollment.infrastructure - persistance layer implementation

As an ORM I choosed Entity Framework Core. I use maybe 1% of EF capabilities, but the integration was so easy I did not wanted to use any other ORM.

As Database engine I picked SQL Lite. This allowed me to not worry about setting up another container when running aplication and also not worry how to run migrations on separate container.

I tried to use JSON:API, unfortunatelly the support in Asp.Net Core is not yet mature. I evaluated all libraries frome [here](https://jsonapi.org/implementations/) but none of them was good enough to use it in this project. I tried to follow JSON:API as close as I could, where it made sense. That means URI structure and status codes. I did not wanted to write my own serializers for request and response bodies.

I could not figure out why Swashbuckle adds ReadOnly properties to POST DTOs, so please be carefull when using
POST /courses or POST /users. Altough Shwashbuckle will allow set id in case of UserDto and id, enrolled in case of CourseDto
The app will return BadRequest if those properties are included.

I also decided to not add versioning to the API at this point as it was not required in the task.
