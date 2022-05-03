using System;
using JetBrains.Annotations;

namespace Network.Models.Other
{
    [Serializable]
    public class User
    {
        public string _id;
        public string email;
        public bool isLogged;
        [CanBeNull] public Position position;
    }
}