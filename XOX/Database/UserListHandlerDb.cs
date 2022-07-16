using FluentResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using XOX.BLObjects;
using XOX.Models;

namespace XOX.Database
{
    public static class UserListHandlerDb
    {
        public async static Task<User> AddUser(User user)
        {
            var contextOptions = new DbContextOptionsBuilder<SessionContext>().Options;
            using (var context = new SessionContext(contextOptions))
            {
                UserModel userModel = await context.Users.FirstOrDefaultAsync(s => s.Id == user.Id);
                if (userModel == null || user.Id == Guid.Empty)
                {
                    userModel = new UserModel(user);
                    await context.AddAsync(userModel);
                    await context.SaveChangesAsync();
                }
                else if (!user.IsEqualByData(userModel))
                {
                    userModel = user.ChangeModel(userModel);
                    context.Update(userModel);
                    await context.SaveChangesAsync();
                }
                user.Id = userModel.Id;
                return user;
            }
        }

        public async static Task<Result<User>> GetUser(Guid userId)
        {
            var contextOptions = new DbContextOptionsBuilder<SessionContext>().Options;
            using (var context = new SessionContext(contextOptions))
            {
                UserModel userModel = await context.Users
                .Include(c => c.UserSessions)
                    .ThenInclude(i => i.Session)
                .FirstOrDefaultAsync(s => s.Id == userId);
                if (userModel == null || userModel.Id == Guid.Empty)
                    return Result.Fail("User not found");
                return new User(userModel);
            }
        }
    }
}
