using System;

namespace Network.Models.RequestEvent
{
    [Serializable]
    public class DisconnectEvent: NetworkEvent
    {
        public DisconnectData data;
    }

    [Serializable]
    public class DisconnectData
    {
        public string authorization;
        public string jwtApi;
        public string userId;
    }
}