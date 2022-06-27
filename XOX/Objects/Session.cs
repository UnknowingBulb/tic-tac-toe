using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XOX.Enums;

namespace XOX.Objects
{
    public class Session
    {
        public User Player1;
        public User Player2;
        public Field Field;
        public SessionState State;

        public int Id;

        public Session()
        {
            State = SessionState.NotStarted;
            Field = new Field();
            Id = SessionListHandler.NewId;
        }
    }
}
