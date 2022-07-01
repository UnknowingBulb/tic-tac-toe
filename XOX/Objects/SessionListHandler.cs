using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XOX.Objects
{
    static public class SessionListHandler
    {
        static private Dictionary<int, Session> Sessions = new Dictionary<int, Session>();

        static private int newId = 0;
        static public int NewId { get => newId++; private set { newId = value; } }

        static public void AddSession(Session session)
        {
            bool exist = Sessions.TryGetValue(session.Id, out Session existing);
            if (exist)
                Sessions[session.Id] = session;
            else
                Sessions.TryAdd(session.Id, session);
        }

        static public Session GetSession(int sessionId)
        {
            Sessions.TryGetValue(sessionId, out Session session);
            return session;
        }

        static public void Remove(int sessionId)
        {
            Sessions.Remove(sessionId);
        }
    }
}
