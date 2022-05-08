using System;
using Network.Models.Other;

namespace Network.Models.RequestEvent
{
    [Serializable]
    public class PlayerMoveEvent: NetworkEvent
    {
        public PlayerMoveData data;
    }
    
    [Serializable]
    public class PlayerMoveData
    {
        public string authorization;
        public string playerId;
        public Position position;
    }
}