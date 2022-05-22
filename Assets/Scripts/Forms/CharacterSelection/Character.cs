using System;

namespace Forms.CharacterSelection
{
    [Serializable]
    public class Character
    {
        public string _id;
        public string nickname;
        public string owner;
        public string __v;
    }

    [Serializable]
    public class Characters
    {
        public Character[] response;
    }
}