using System;
using XOX.Enums;

namespace XOX.BLObjects
{
    public class SessionDto
    {
        public int Id;
        public User Player1;
        public User Player2;
        public Field Field;
        public SessionState State;
        public bool IsActivePlayer1;


        public SessionDto(Session session)
        {
            State = session.State;
            Field = session.Field;
            Id = session.Id;
            Player1 = UserListHandler.GetUser(session.Player1Id);
            Player2 = session.Player2Id == null ? null: UserListHandler.GetUser(session.Player2Id);
            IsActivePlayer1 = session.IsActivePlayer1;
        }
    }
}
