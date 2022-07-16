using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using XOX.BLObjects;

namespace XOX.Models
{
    public class SessionModel : IDbModel
    {
        public int? Id { get; set; }

        //public Guid Player1Id { get; set; }

        //public Guid Player2Id { get; set; }

        public string Field { get; set; }

        public int State { get; set; }

        //public bool IsActivePlayer1 { get; set; }
        public DateTime StartDate { get; set; }

        public ICollection<UserSessionsModel> UserSessions { get; set; }

        public SessionModel() { }

        public SessionModel(Session session)
        {
            State = (int)session.State;
            Field = JsonConvert.SerializeObject(session.Field);
            StartDate = DateTime.UtcNow;
        }
    }
}
