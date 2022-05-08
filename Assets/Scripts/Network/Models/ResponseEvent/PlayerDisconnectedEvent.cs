using System;
using System.Collections.Generic;
using Network.Models.Other;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class PlayerDisconnectedEvent: NetworkEvent
    {
        public PlayerDisconnectedData data;
    }

    [Serializable]
    public class PlayerDisconnectedData
    {
        public Player player;
    }
}