using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.Objects
{
    public class Session
    {
        public User Player1;
        public User Player2;
        public Field Field;
        public int Id;

        public Session()
        {
            Field = new Field();
            Id = SessionListHandler.NewId;
        }
    }
}
