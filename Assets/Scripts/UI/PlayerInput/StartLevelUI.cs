using System.Collections.Generic;
using Model;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI.PlayerInput
{
    public class StartLevelUI : MonoBehaviour
    {
        private string _levelName;

        [Header("Reference to text fields for player names")]
        [SerializeField]
        private List<TextMeshProUGUI> _inputPlayersName = new List<TextMeshProUGUI>();
        
        public void SetLevelName(string levelName)
        {
            _levelName = levelName;
        }
        
        public void StartLevel()
        {
            var player1Name = _inputPlayersName[0].text+_inputPlayersName[1].text+_inputPlayersName[2].text;
            var player2Name = _inputPlayersName[3].text+_inputPlayersName[4].text+_inputPlayersName[5].text;

            // Check if any player name contains only "_" characters
            if (player1Name.Contains("_") || player2Name.Contains("_"))
            {
                Debug.LogError("Player names cannot contain only '_' characters");
                return;
            }
            
            InputPlayersName.Player1Name = player1Name;
            InputPlayersName.Player2Name = player2Name;
            SceneManager.LoadScene(_levelName);
        }
    }
}