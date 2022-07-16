using System;
using XOX.BLObjects;

namespace XOX.Models
{
    /// <summary>
    /// Model of Session - User Relation table
    /// </summary>
    public class UserSessionsModel : IDbModel
    {
        public Guid UserModelId { get; set; }

        public int? SessionModelId { get; set; }

        public bool IsActive { get; set; }

        public SessionModel Session { get; set; }

        public UserModel User { get; set; }

        public UserSessionsModel() { }

        public UserSessionsModel(Guid userId, int? sessionId, bool isActive)
        {
            UserModelId = userId;
            SessionModelId = sessionId;
            IsActive = isActive;
        }
    }
}