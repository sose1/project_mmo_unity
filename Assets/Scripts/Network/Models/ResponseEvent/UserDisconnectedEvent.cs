using System;
using System.Collections.Generic;

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
        public List<Other.User> users;
    }
}