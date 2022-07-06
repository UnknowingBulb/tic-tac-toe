using System;
using XOX.Enums;

namespace XOX.BLObjects
{
    public class Session
    {
        public Guid Player1Id;
        public Guid Player2Id;
        public Field Field;
        public SessionState State;
        public bool IsActivePlayer1;

        public int Id;

        public Session(User player)
        {
            State = SessionState.NotStarted;
            Field = new Field();
            Id = SessionListHandler.NewId;
            Player1Id = player.Id;
            Player2Id = Guid.Empty;
            IsActivePlayer1 = true;
        }
    }
}
