using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XOX.BLObjects;

namespace XOX.Models
{
    public class SessionModel
    {
        public int Id;
        public Guid Player1Id;
        public Guid Player2Id;
        public string Field;
        public int State;
        public bool IsActivePlayer1;

        public SessionModel() { }

        public SessionModel(Session session)
        {
            State = (int)session.State;
            Field = JsonConvert.SerializeObject(session.Field);
            Player1Id = session.Player1Id;
            Player2Id = session.Player2Id;
            IsActivePlayer1 = session.IsActivePlayer1;
        }
        
        public Session toSession()
        {
            return new Session(Id, Player1Id, Player2Id, Field, State, IsActivePlayer1);
        }
    }
}
