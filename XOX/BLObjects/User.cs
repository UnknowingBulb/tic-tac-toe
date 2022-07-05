using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.BLObjects
{
    public class User
    {
        public Guid Id;
        public string Name;
        public string Mark;

        public User(Guid id, string name, string mark)
        {
            Id = id;
            Name = name;
            Mark = mark;
        }

        // TODO: создать нормально, а не как сейчас
        public User(Guid id, bool isFirst = false)
        {
            Id = id;
            Name = isFirst ? "Player1" : "Player2";
            Mark = isFirst ? "🙄" : "😡";
        }
    }
}
