using XOX.Enums;

namespace XOX.Objects
{
    public class Session
    {
        public User Player1;
        public User Player2;
        public Field Field;
        public SessionState State;
        public bool IsActivePlayer1;

        public int Id;

        public Session(User player)
        {
            State = SessionState.NotStarted;
            Field = new Field();
            Id = SessionListHandler.NewId;
            Player1 = player;
            IsActivePlayer1 = true;
        }
    }
}
