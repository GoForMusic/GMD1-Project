using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        
        public void LoadDataFromJson(string jsonFileName)
        {
            string path = Application.dataPath + $"/{jsonFileName}.json";

            if (File.Exists(path))
            {
                string jsonData = File.ReadAllText(path);
                _playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);
            }
            else
            {
                CreateNewJsonFile(jsonFileName);
                string jsonData = File.ReadAllText(path);
                _playerDataList = JsonUtility.FromJson<PlayerDataList>(jsonData);
            }

            SortDataByScore();
            PopulateUI();
        }
        
        private void CreateNewJsonFile(string jsonFileName)
        {
            string path = Application.dataPath + $"/{jsonFileName}.json";

            // Create a sample list of player data
            List<PlayerData> sampleData = new List<PlayerData>
            {
                new PlayerData { name = "JOH", score = 85, team = "team1" },
                new PlayerData { name = "ALI", score = 92, team = "team1" },
                new PlayerData { name = "BOB", score = 78, team = "team2" }
            };

            // Wrap the list in a PlayerDataList object
            PlayerDataList dataWrapper = new PlayerDataList();
            dataWrapper.playerDataList = sampleData;
            
            // Serialize the sample data list to JSON
            string jsonData = JsonUtility.ToJson(dataWrapper);
            
            // Write the JSON data to the file
            try
            {
                // Write the JSON data to the file
                File.WriteAllText(path, jsonData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Failed to write JSON file: {e.Message}");
                return;
            }
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