using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.Objects
{
    //TODO: заменить на нормальное, здесь временно сделано, чтобы было
    static public class UserListHandler
    {
        static private Dictionary<Guid, User> Users = new Dictionary<Guid, User>();

        static public User AddUser(User user)
        {
            bool exist = Users.TryGetValue(user.Id, out User existing);
            if (exist)
                //TODO: это фигня, но надо обновлять в списке (или БД), если уже есть
                Users[user.Id] = user;
            else
                Users.TryAdd(user.Id, user);
            return user;

        }

        static public User GetUser(Guid userId)
        {
            Users.TryGetValue(userId, out User user);
            return user;
        }

        static public void Remove(Guid userId)
        {
            Users.Remove(userId);
        }
    }
}
