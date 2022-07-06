using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lib.AspNetCore.ServerSentEvents;

namespace XOX.Services
{
    internal abstract class NotificationsServiceBase
    {
        #region Fields
        private INotificationsServerSentEventsService _notificationsServerSentEventsService;
        #endregion

        #region Constructor
        protected NotificationsServiceBase(INotificationsServerSentEventsService notificationsServerSentEventsService)
        {
            _notificationsServerSentEventsService = notificationsServerSentEventsService;
        }
        #endregion

        #region Methods
        protected Task SendSseEventAsync(string notification, string group)
        {
            return _notificationsServerSentEventsService.SendEventAsync(new ServerSentEvent
            {
                Data = new List<string>(notification.Split(new string[] { "\r\n", "\n" }, StringSplitOptions.None))
            });
        }

        public ServerSentEventsAddToGroupResult AddUserToGroup1(Guid userId, string groupName)
        {
            IServerSentEventsClient user = _notificationsServerSentEventsService.GetClient(userId);
            return _notificationsServerSentEventsService.AddToGroup(groupName, user);

        }

        public IReadOnlyCollection<IServerSentEventsClient> GetUsers1()
        {
            return _notificationsServerSentEventsService.GetClients();
        }
        public IReadOnlyCollection<IServerSentEventsClient> GetUsers1(string groupName)
        {
            return _notificationsServerSentEventsService.GetClients(groupName);
        }
        #endregion
    }
}