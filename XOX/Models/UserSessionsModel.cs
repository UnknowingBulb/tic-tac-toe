using System;

namespace XOX.Models
{
    /// <summary>
    /// Model of Session - User Relation table
    /// </summary>
    public class UserSessionsModel
    {
        public Guid UserModelId { get; set; }
        public int SessionModelId { get; set; }

        public bool IsActive { get; set; }

        public UserSessionsModel() { }

        public UserSessionsModel(Guid userId, int sessionId, bool isActive)
        {
            UserModelId = userId;
            SessionModelId = sessionId;
            IsActive = isActive;
        }
    }
}