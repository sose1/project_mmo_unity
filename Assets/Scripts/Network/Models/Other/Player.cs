using System;
using JetBrains.Annotations;

namespace Network.Models.Other
{
    [Serializable]
    public class Player
    {
        public string _id;
        public string email;
        public string nickname;
        public bool isLogged;
        [CanBeNull] public Position position;
    }
}