using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using XOX.BLObjects;

namespace XOX.Models
{
    public class SessionModel
    {
        public int Id { get; set; }
        public Guid Player1Id { get; set; }
        public Guid Player2Id { get; set; }
        public string Field { get; set; }
        public int State { get; set; }
        public bool IsActivePlayer1 { get; set; }

        public SessionModel() { }

        public SessionModel(Session session)
        {
            State = (int)session.State;
            Field = JsonConvert.SerializeObject(session.Field);
            Player1Id = session.Player1Id;
            Player2Id = session.Player2Id;
            IsActivePlayer1 = session.IsActivePlayer1;
        }
        
        public Session ToSession()
        {
            return new Session(Id, Player1Id, Player2Id, Field, State, IsActivePlayer1);
        }

        public SessionModel ChangeWithSession(Session session)
        {
            State = (int)session.State;
            Field = JsonConvert.SerializeObject(session.Field);
            Player1Id = session.Player1Id;
            Player2Id = session.Player2Id;
            IsActivePlayer1 = session.IsActivePlayer1;
            return this;
        }
    }
}
