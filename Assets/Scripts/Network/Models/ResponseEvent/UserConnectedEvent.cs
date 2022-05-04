using System;
using Network.Models.Other;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class UserConnectedEvent: NetworkEvent
    {
        public UserConnectedData data;
    }

    [Serializable]
    public class UserConnectedData
    {
        public User user;
    }
}