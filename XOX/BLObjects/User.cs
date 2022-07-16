using emoji_dotnet;
using FluentResults;
using System;
using System.Linq;
using System.Threading.Tasks;
using XOX.Database;
using XOX.Enums;
using XOX.Models;

namespace XOX.BLObjects
{
    public class User : IDbObject<User, UserModel, Guid>
    {
        public Guid Id;
        public string Name;
        public string Mark;

        public bool HasActiveSessions { get; private set; }

        public User() { }

        public User(Guid id, string name, string mark)
        {
            Id = id;
            Name = name;
            Mark = mark;
        }

        public User(UserModel model)
        {
            if (model == null)
                return;
            Id = model.Id;
            Name = model.Name;
            Mark = model.Mark;
            HasActiveSessions = model.UserSessions.Any(s => (s.Session.State == (int)SessionState.InProgress) ||
                (s.Session.State == (int)SessionState.NotStarted));
        }

        public User(Guid id)
        {
            Id = id;
            Name = RandomFriendlyNameGenerator.NameGenerator.Identifiers.Get();
            Mark = EmojiUid.Generate(1);;
        }

        public bool IsEqualByData(UserModel model)
        {
            return (Name == model.Name && Mark == model.Mark);
        }

        public UserModel ChangeModel(UserModel model)
        {
            Name = model.Name;
            Mark = model.Mark;
            return model;
        }

        public async Task<Result<User>> Get(Guid id)
        {
            return await UserListHandlerDb.GetUser(id);
        }

        public async Task<Result<User>> Save()
        {
            return await UserListHandlerDb.AddUser(this); ;
        }

        public async static Task<Result<User>> GetOrCreate(Guid id)
        {
            var userResult = await new User().Get(id);

            if (userResult.IsSuccess)
                return userResult.Value;

            var user = new User(id);
            return await user.Save();
        }
    }
}