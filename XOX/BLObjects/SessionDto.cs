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


        public SessionDto(Session session)
        {
            State = session.State;
            Field = session.Field;
            Id = session.Id;
            Player1 = session.Player1;
            Player2 = session.Player2;
            IsActivePlayer1 = session.IsActivePlayer1;
        }
    }
}
