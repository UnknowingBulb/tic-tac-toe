using System;
using XOX.Enums;

namespace XOX.BLObjects
{
    public class SessionDto
    {
        public int? Id;
        public User Player1;
        public User Player2;
        public Field Field;
        public SessionState State;
        public bool IsActivePlayer1;


        public SessionDto(Session session, User player1, User player2)
        {
            State = session.State;
            Field = session.Field;
            Id = session.Id;
            Player1 = player1;
            Player2 = player2;
            IsActivePlayer1 = session.IsActivePlayer1;
        }
    }
}
