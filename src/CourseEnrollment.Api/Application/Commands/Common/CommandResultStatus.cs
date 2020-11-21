namespace CourseEnrollment.Api.Application.Commands.Common
{
    public enum CommandResultStatus
    {
        Success,
        DuplicatedEntity,
        Deleted,
        NotFound
    }
}