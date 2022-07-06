using Lib.AspNetCore.ServerSentEvents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOX.Services
{
    internal class LocalNotificationsService : NotificationsServiceBase, INotificationsService
    {
        #region Constructor
        public LocalNotificationsService(INotificationsServerSentEventsService notificationsServerSentEventsService)
            : base(notificationsServerSentEventsService)
        { }
        #endregion

        #region Methods
        public Task SendNotificationAsync(string notification, string group)
        {
            return SendSseEventAsync(notification, group);
        }

        public ServerSentEventsAddToGroupResult AddUserToGroup(Guid userId, string groupName)
        {
            return AddUserToGroup1(userId, groupName);

        }

        public IReadOnlyCollection<IServerSentEventsClient> GetUsers()
        {
            return GetUsers1();
        }
        public IReadOnlyCollection<IServerSentEventsClient> GetUsers(string groupName)
        {
            return GetUsers1(groupName);
        }
        #endregion
    }
}
