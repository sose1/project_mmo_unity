using System;
using Network.Models.Other;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class PlayerConnectedEvent: NetworkEvent
    {
        public PlayerConnectedData data;
    }

    [Serializable]
    public class PlayerConnectedData
    {
        public Player player;
    }
}