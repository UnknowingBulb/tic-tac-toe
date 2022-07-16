using FluentResults;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Threading.Tasks;
using XOX.Database;
using XOX.Enums;
using XOX.Models;

namespace XOX.BLObjects
{
    public class Session : IDbObject<Session, SessionModel, int?>
    {
        public int? Id;
        public User Player1;
        public User Player2;
        public Field Field;
        public SessionState State;
        public bool IsActivePlayer1;

        public Session() { }

        public Session(User player)
        {
            State = SessionState.NotStarted;
            Field = new Field();
            Id = null;
            Player1 = player;
            Player2 = null;
            IsActivePlayer1 = true;
        }

        public Session(int? id, User player1, User player2, string field, int state, bool? isActivePlayer1)
        {
            Id = id;
            Player1 = player1;
            Player2 = player2;
            Field = JsonConvert.DeserializeObject<Field>(field);
            State = (SessionState)state;
            IsActivePlayer1 = isActivePlayer1 ?? false;
        }

        public Session(int? id, UserModel player1, UserModel player2, string field, int state, bool? isActivePlayer1)
        {
            Id = id;
            Player1 = new User(player1);
            Player2 = new User(player2);
            Field = JsonConvert.DeserializeObject<Field>(field);
            State = (SessionState)state;
            IsActivePlayer1 = isActivePlayer1 ?? false;
        }

        public bool IsEqualByData(SessionModel model)
        {
            return ((int)State == model.State &&
                JsonConvert.SerializeObject(Field) == model.Field &&
                Player1.Id == model.UserSessions.ElementAtOrDefault(0)?.UserModelId &&
                Player2.Id == model.UserSessions.ElementAtOrDefault(1)?.UserModelId &&
                IsActivePlayer1 == model.UserSessions.ElementAt(0).IsActive);
        }

        public async Task<Result<Session>> Get(int? id)
        {
            return await SessionListHandlerDb.GetSession(id);
        }

        public SessionModel ChangeModel(SessionModel model)
        {
            model.Field = JsonConvert.SerializeObject(Field);
            model.State = (int)State;
            return model;
        }

        public async Task<Result<Session>> Save()
        {
            return await SessionListHandlerDb.AddSession(this);
        }
    }
}
