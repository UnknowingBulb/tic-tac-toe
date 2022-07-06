using Lib.AspNetCore.ServerSentEvents;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace XOX.Services
{
    public interface INotificationsService
    {
        Task SendNotificationAsync(string notification, string group);

        ServerSentEventsAddToGroupResult AddUserToGroup(Guid userId, string groupName);

        IReadOnlyCollection<IServerSentEventsClient> GetUsers();

        IReadOnlyCollection<IServerSentEventsClient> GetUsers(string groupName);
    }
}
