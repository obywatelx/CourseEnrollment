using System;
using System.Threading.Tasks;

namespace CourseEnrollment.Domain.Model
{
    public interface IUserRepository
    {
        Task<User> AddAsync(User user);
        Task UpdateAsync(User user);

        Task<User> GetByUserIdAsync(Guid userId);

        Task DeleteAsync(User user);

        Task<bool> UserExistsAsync(string emailAddress);
    }
}