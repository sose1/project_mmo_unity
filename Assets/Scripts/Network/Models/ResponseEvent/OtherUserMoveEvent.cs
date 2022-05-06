using System;
using Network.Models.Other;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class OtherUserMoveEvent : NetworkEvent
    {
        public OtherUserMoveData data;
    }

    [Serializable]
    public class OtherUserMoveData
    {
        public string userId;
        public Position position;
        public string receivedTime;
    }
}