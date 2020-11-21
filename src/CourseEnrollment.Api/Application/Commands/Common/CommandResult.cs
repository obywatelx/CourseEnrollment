namespace CourseEnrollment.Api.Application.Commands.Common
{
    public class CommandResult<T> where T : class
    {
        public CommandResultStatus Status { get; set; }

        public T Entity { get; set; }
        public string Message { get; set; }
    }
}