using System;
using Microsoft.AspNetCore.Http;
using Lib.AspNetCore.ServerSentEvents;
using System.Collections.Generic;

namespace XOX.Services
{
    internal class ClientService : IClientService
    {
        private INotificationsServerSentEventsService _notificationsServerSentEventsService;

        public ClientService(INotificationsServerSentEventsService notificationsServerSentEventsService)
        {
            _notificationsServerSentEventsService = notificationsServerSentEventsService;
        }

        public ServerSentEventsAddToGroupResult? AddUserToGroup(Guid userId, string groupName)
        {
            if (userId == Guid.Empty)
                return null;
            IServerSentEventsClient user = _notificationsServerSentEventsService.GetClient(userId);
            if (user == null)
                return null;
            return _notificationsServerSentEventsService.AddToGroup(groupName, user);
        }

        public IReadOnlyCollection<IServerSentEventsClient> GetUsers()
        {
            return _notificationsServerSentEventsService.GetClients();
        }
        public IReadOnlyCollection<IServerSentEventsClient> GetUsers(string groupName)
        {
            return _notificationsServerSentEventsService.GetClients(groupName);
        }
    }
}
