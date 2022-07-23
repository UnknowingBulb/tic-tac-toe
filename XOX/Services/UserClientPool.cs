using System;
using Microsoft.AspNetCore.Http;
using Lib.AspNetCore.ServerSentEvents;
using System.Collections.Generic;
using System.Linq;

namespace XOX.Services
{
    internal class UserClientPool
    {
        private static Dictionary<Guid, Guid> _clientUserPairs = new Dictionary<Guid, Guid>();
        public UserClientPool() { }

        public static void AddClient(Guid client, Guid user)
        {
            if (_clientUserPairs.ContainsKey(client))
                _clientUserPairs[client] = user;
            else
                _clientUserPairs.Add(client, user);
        }

        public static Guid GetLastClientId(Guid user)
        {
            if (!_clientUserPairs.ContainsValue(user))
                return Guid.Empty;
            return _clientUserPairs.LastOrDefault(x => x.Value == user).Key;
        }

        public static IEnumerable<Guid> GetAllClientId(Guid user)
        {
            if (!_clientUserPairs.ContainsValue(user))
                return null;
            return _clientUserPairs.Where(x => x.Value == user).Select(x => x.Key);
        }

    }
}
