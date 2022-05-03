using System;
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
    }
}