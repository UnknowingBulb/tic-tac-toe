using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.Objects
{
    //TODO: заменить на нормальное, здесь временно сделано, чтобы было
    static public class UserListHandler
    {
        static private Dictionary<int, User> Users = new Dictionary<int, User>();

        static private int newId = 0;
        static public int NewId { get => newId++; private set { newId = value; } }

        static public void AddUser(User user)
        {
            bool exist = Users.TryGetValue(user.Id, out User existing);
            if (exist)
                existing = user;
            else
                Users.TryAdd(user.Id, user);
        }

        static public User GetUser(int userId)
        {
            Users.TryGetValue(userId, out User user);
            return user;
        }

        static public void Remove(int userId)
        {
            Users.Remove(userId);
        }
    }
}
