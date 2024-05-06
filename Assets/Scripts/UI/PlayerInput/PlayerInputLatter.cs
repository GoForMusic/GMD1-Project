using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.PlayerInput
{
    /// <summary>
    /// Manages the input of alphabetical characters for player names in a UI component.
    /// </summary>
    public class PlayerInputLatter : MonoBehaviour
    {
        private List<char> _nameCharacters = new List<char>();

        /// <summary>
        /// Initializes the list of characters with an underscore followed by the uppercase English alphabets.
        /// </summary>
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

        /// <summary>
        /// Increments the current letter displayed in the UI, wrapping around to the start if necessary.
        /// </summary>
        /// <param name="latter">The UI text component displaying the current letter.</param>
        public void ChangeLatterUp(TextMeshProUGUI latter)
        {
            int letterIndex = _nameCharacters.IndexOf(latter.text[0]);

            letterIndex++;
            if (letterIndex >= _nameCharacters.Count)
                letterIndex = 0;

            UpdateFirstLetterDisplay(latter,_nameCharacters[letterIndex]);
        }

        /// <summary>
        /// Decrements the current letter displayed in the UI, wrapping around to the end if necessary.
        /// </summary>
        /// <param name="latter">The UI text component displaying the current letter.</param>
        public void ChangeLatterDown(TextMeshProUGUI latter)
        {
            int letterIndex = _nameCharacters.IndexOf(latter.text[0]);
            letterIndex--;
            if (letterIndex < 0)
                letterIndex = _nameCharacters.Count - 1;

            UpdateFirstLetterDisplay(latter,_nameCharacters[letterIndex]);
        }

        /// <summary>
        /// Updates the text of the provided TextMeshProUGUI component to display the specified character.
        /// </summary>
        /// <param name="latter">The TextMeshProUGUI component whose text will be updated.</param>
        /// <param name="x">The character to display.</param>
        private void UpdateFirstLetterDisplay(TextMeshProUGUI latter,char x)
        {
            latter.text = x.ToString();
        }
    }
}