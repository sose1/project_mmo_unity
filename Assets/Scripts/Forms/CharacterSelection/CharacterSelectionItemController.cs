using System;
using TMPro;
using UnityEngine;

namespace Forms.CharacterSelection
{
    public class CharacterSelectionItemController: MonoBehaviour
    {
        public TMP_Text NicknameText;
        public string Nickname;
    
        private void Start()
        {
            NicknameText.text = Nickname;
        }
        
    }
}