using System;
using System.Collections.Generic;
using Interfaces.File;
using Model;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private UIManager player1UI;
        [SerializeField]
        private UIManager player2UI;
        [SerializeField]
        private string jsFile;

        private IScoreboardDataManager _scoreboardDataManager;
        private TimerController _timerController;
        
        private void Awake()
        {
            // Initialize the ScoreboardDataManager
            _scoreboardDataManager = new JsonScoreboardDataManager();
            _timerController = FindObjectOfType<TimerController>();
            _timerController.GameOverEvent += OnGameOverEvent;
        }

        /// <summary>
        /// Handles actions to perform on game over, including saving player data and loading the main menu.
        /// </summary>
        private void OnGameOverEvent()
        {
            PlayerDataList playerDataList = new PlayerDataList
            {
                playerDataList = new List<PlayerData> {new PlayerData
                {
                    name = InputPlayersName.Player1Name,
                    score = player1UI.GetCurrentScore(),
                    team = "team1"
                },
                new PlayerData{
                    name = InputPlayersName.Player2Name,
                    score = player2UI.GetCurrentScore(),
                    team = "team2"
                }}
            };
            _scoreboardDataManager.SaveData(playerDataList,jsFile);
            SceneManager.LoadScene("MainMenu");
        }

        /// <summary>
        /// Manually triggers game over actions, specifically loading the main menu.
        /// </summary>
        public void OnManualGameOverEvent()
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}