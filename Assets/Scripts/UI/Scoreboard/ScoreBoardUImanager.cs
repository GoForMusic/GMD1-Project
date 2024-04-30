using System.Collections.Generic;
using System.IO;
using System.Linq;
using Interfaces.File;
using Model;
using UnityEngine;

namespace UI.Scoreboard
{
    public class ScoreBoardUImanager : MonoBehaviour
    {
        [SerializeField] 
        private GameObject _scoreboardEntryPrefab;
        private PlayerDataList _playerDataList = new PlayerDataList();
        [SerializeField] private Transform team1ScoreboardParent;
        [SerializeField] private Transform team2ScoreboardParent;
        
        private IScoreboardDataManager _dataManager;
        
        public void LoadDataFromJson(string jsonFileName)
        {
            _dataManager = new JsonScoreboardDataManager();
            _playerDataList = _dataManager.LoadData(jsonFileName);
            SortDataByScore();
            PopulateUI();
        }
        
        private void SortDataByScore()
        {
            if (_playerDataList != null && _playerDataList.playerDataList != null)
            {
                _playerDataList.playerDataList = _playerDataList.playerDataList.OrderByDescending(player => player.score).ToList();
            }
        }
        
        private void PopulateUI()
        {
            if (_playerDataList == null || _playerDataList.playerDataList == null)
            {
                Debug.LogError("Player data list is null or empty.");
                return;
            }
            
            // Separate playerDataList into two lists based on team
            List<PlayerData> team1Players = _playerDataList.playerDataList.Where(player => player.team == "team1").ToList();
            List<PlayerData> team2Players = _playerDataList.playerDataList.Where(player => player.team == "team2").ToList();
            
            // Initialize position counters for each team
            int team1Position = 1;
            int team2Position = 1;
            
            // Define initial Y positions
            float initialYPosition = 0f;
            float yDifference = 20.4f;
            
            // Clear existing entries before populating UI for Team 1
            foreach (Transform child in team1ScoreboardParent)
            {
                Destroy(child.gameObject);
            }
            
            // Clear existing entries before populating UI for Team 1
            foreach (Transform child in team2ScoreboardParent)
            {
                Destroy(child.gameObject);
            }
            
            // Populate UI for Team 1
            foreach (var playerData in team1Players)
            {
                GameObject scoreboardEntry = Instantiate(_scoreboardEntryPrefab, team1ScoreboardParent, false);
                // Calculate Y position
                scoreboardEntry.transform.localPosition = new Vector3(scoreboardEntry.transform.localPosition.x, initialYPosition - yDifference * team1Position, scoreboardEntry.transform.localPosition.z);
                scoreboardEntry.GetComponent<ScoreboardEntry>().Initialize(team1Position, playerData);
                team1Position++;
            }

            // Populate UI for Team 2
            foreach (var playerData in team2Players)
            {
                GameObject scoreboardEntry = Instantiate(_scoreboardEntryPrefab, team2ScoreboardParent, false);
                // Calculate Y position
                scoreboardEntry.transform.localPosition = new Vector3(scoreboardEntry.transform.localPosition.x, initialYPosition - yDifference * team2Position, scoreboardEntry.transform.localPosition.z);
                scoreboardEntry.GetComponent<ScoreboardEntry>().Initialize(team2Position, playerData);
                team2Position++;
            }
        }
    }
}