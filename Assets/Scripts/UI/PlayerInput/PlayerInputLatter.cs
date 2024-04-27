using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.PlayerInput
{
    public class PlayerInputLatter : MonoBehaviour
    {
        private List<char> _nameCharacters = new List<char>();

        void Start()
        {
            // Add the initial '_' character
            _nameCharacters.Add('_');

            // Add uppercase letters from A to Z
            for (char c = 'A'; c <= 'Z'; c++)
            {
                _nameCharacters.Add(c);
            }
        }

        public void ChangeLatterUp(TextMeshProUGUI latter)
        {
            int letterIndex = _nameCharacters.IndexOf(latter.text[0]);

            letterIndex++;
            if (letterIndex >= _nameCharacters.Count)
                letterIndex = 0;

            UpdateFirstLetterDisplay(latter,_nameCharacters[letterIndex]);
        }

        public void ChangeLatterDown(TextMeshProUGUI latter)
        {
            int letterIndex = _nameCharacters.IndexOf(latter.text[0]);
            letterIndex--;
            if (letterIndex < 0)
                letterIndex = _nameCharacters.Count - 1;

            UpdateFirstLetterDisplay(latter,_nameCharacters[letterIndex]);
        }

        private void UpdateFirstLetterDisplay(TextMeshProUGUI latter,char x)
        {
            latter.text = x.ToString();
        }
    }
}