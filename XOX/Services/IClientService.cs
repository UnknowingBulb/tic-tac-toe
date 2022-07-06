using System;
using Lib.AspNetCore.ServerSentEvents;
using System.Collections.Generic;

namespace XOX.Services
{
    public interface IClientService
    {
        public ServerSentEventsAddToGroupResult AddUserToGroup(Guid userId, string groupName);

        public IReadOnlyCollection<IServerSentEventsClient> GetUsers();

        public IReadOnlyCollection<IServerSentEventsClient> GetUsers(string groupName);
    }
}
