using System;

namespace Network.Models.RequestEvent
{
    [Serializable]
    public class ConnectEvent : NetworkEvent
    {
        public Data data;
    }

    [Serializable]
    public class Data
    {
        public string jwtApi;
    }
}