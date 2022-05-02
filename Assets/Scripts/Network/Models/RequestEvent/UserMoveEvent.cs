using System;
using Network.Models.Other;

namespace Network.Models.RequestEvent
{
    [Serializable]
    public class UserMoveEvent: NetworkEvent
    {
        public UserMoveData data;
    }
    
    [Serializable]
    public class UserMoveData
    {
        public string authorization;
        public string userId;
        public Position position;
    }
}