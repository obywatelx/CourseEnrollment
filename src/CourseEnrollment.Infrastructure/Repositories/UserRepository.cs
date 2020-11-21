using System.Threading;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CourseEnrollment.Domain.Model;

namespace CourseEnrollment.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        public CourseEnrollmentContext Context { get; }

        public UserRepository(CourseEnrollmentContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<User> AddAsync(User user)
        {
            await Context.Users.AddAsync(user);
            await Context.SaveChangesAsync(CancellationToken.None);
            return user;
        }

        public async Task DeleteAsync(User user)
        {
            var userToRemove = Context.Users.Where(u => u.Id == user.Id).First();
            Context.Users.Remove(userToRemove);
            await Context.SaveChangesAsync(CancellationToken.None);
        }

        public async Task<User> GetByUserIdAsync(Guid userId)
        {
            var user = await Context.Users.Include(u => u.Courses).SingleOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                return null;
            }
            return user;
        }

        public async Task<bool> UserExistsAsync(string emailAddress)
        {
            return await Context.Users.AnyAsync(user => user.Email == emailAddress);
        }

        public async Task UpdateAsync(User user)
        {
            Context.Entry(user).State = EntityState.Modified;
            await Context.SaveChangesAsync(CancellationToken.None);
        }
    }
}