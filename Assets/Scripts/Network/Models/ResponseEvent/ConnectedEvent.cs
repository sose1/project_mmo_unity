using System;

namespace Network.Models.ResponseEvent
{
    [Serializable]
    public class ConnectedEvent : NetworkEvent
    {
        public Data data;

    }

    [Serializable]
    public class Data
    {
        public string jwtServer;
        public User user;
    }

    [Serializable]
    public class User
    {
        public string _id;
        public string email;
        public string password;
    }
}