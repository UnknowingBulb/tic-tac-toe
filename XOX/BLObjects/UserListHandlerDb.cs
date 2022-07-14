using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using XOX.Database;
using XOX.Models;

namespace XOX.BLObjects
{
    //TODO: заменить на нормальное, здесь временно сделано, чтобы было

    public class UserListHandlerDb
    {
        private readonly SessionContext _context;
        public UserListHandlerDb(SessionContext context)
        {
            _context = context;
        }

        public async Task<User> AddUser(User user)
        {
            UserModel userModel = await _context.Users.FirstOrDefaultAsync(s => s.Id == user.Id);
            if (userModel == null || user.Id == Guid.Empty)
            {
                userModel = new UserModel(user);
                await _context.AddAsync(userModel);
                await _context.SaveChangesAsync();
            }
            else if (!userModel.IsEqualByData(user))
            {
                userModel = userModel.ChangeWithUser(user);
                _context.Update(userModel);
                await _context.SaveChangesAsync();
            }
            return userModel.ToUser();
        }

        public async Task<User> GetUser(Guid userId)
        {
            UserModel userModel = await _context.Users.FirstOrDefaultAsync(s => s.Id == userId);
            return userModel?.ToUser();
        }
    }
}
