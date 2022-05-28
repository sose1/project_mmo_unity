using System;
using Network.Models.Other;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class OtherPlayerMoveEvent : NetworkEvent
    {
        public OtherPlayerMoveData data;
    }

    [Serializable]
    public class OtherPlayerMoveData
    {
        public string playerId;
        public Position position;
        public string animationState;
    }
}