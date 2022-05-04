using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Network.Models.Other;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class ConnectedEvent : NetworkEvent
    {
        public ConnectedData data;

    }

    [Serializable]
    public class ConnectedData
    {
        public string jwtServer;
        public User user;
        [CanBeNull] public List<User> otherUsers;
    }
}