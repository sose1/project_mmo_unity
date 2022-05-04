using System;
using System.Collections.Generic;
using Network.Models.Other;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class UserDisconnectedEvent: NetworkEvent
    {
        public UserDisconnectedData data;
    }

    [Serializable]
    public class UserDisconnectedData
    {
        public User user;
    }
}