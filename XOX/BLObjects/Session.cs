using Newtonsoft.Json;
using System;
using XOX.Enums;

namespace XOX.BLObjects
{
    public class Session
    {
        public int? Id;
        public Guid Player1Id;
        public Guid Player2Id;
        public Field Field;
        public SessionState State;
        public bool IsActivePlayer1;

        public Session(User player)
        {
            State = SessionState.NotStarted;
            Field = new Field();
            Id = null;
            Player1Id = player.Id;
            Player2Id = Guid.Empty;
            IsActivePlayer1 = true;
        }

        public Session(int id, Guid player1Id, Guid player2Id, string field, SessionState state, bool isActivePlayer1)
        {
            Id = id;
            Player1Id = player1Id;
            Player2Id = player2Id;
            Field = JsonConvert.DeserializeObject<Field>(field);
            State = state;
            IsActivePlayer1 = isActivePlayer1;
        }
    }
}
