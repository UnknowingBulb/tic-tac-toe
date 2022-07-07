using emoji_dotnet;
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
        //TODO: do calculations when db will be done
        public bool HasActiveSessions = false;

        public User(Guid id, string name, string mark)
        {
            Id = id;
            Name = name;
            Mark = mark;
        }

        // TODO: создать нормально, а не как сейчас
        public User(Guid id)
        {
            Id = id;
            Name = RandomFriendlyNameGenerator.NameGenerator.Identifiers.Get();
            Mark = EmojiUid.Generate(1);;
        }
    }
}
