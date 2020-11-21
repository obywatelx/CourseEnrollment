FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY ./CourseEnrollment.Api/*.csproj ./CourseEnrollment.Api/
COPY ./CourseEnrollment.Domain/*.csproj ./CourseEnrollment.Domain/
COPY ./CourseEnrollment.Infrastructure/*.csproj ./CourseEnrollment.Infrastructure/
RUN dotnet restore ./CourseEnrollment.Api/CourseEnrollment.Api.csproj

# Copy everything else and build
COPY . ./
RUN dotnet build ./CourseEnrollment.Api/CourseEnrollment.Api.csproj -c Debug -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:5.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "CourseEnrollment.Api.dll"]
