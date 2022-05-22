using System;

namespace Network.Models.RequestEvent
{
    [Serializable]
    public class ConnectEvent : NetworkEvent
    {
        public ConnectData data;
    }

    [Serializable]
    public class ConnectData
    {
        public string jwtApi;
        public string characterId;
    }
}