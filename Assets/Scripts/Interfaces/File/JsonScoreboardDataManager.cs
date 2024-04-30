using System.Linq;
using Model;
using UnityEngine;

namespace Interfaces.File
{
    public class JsonScoreboardDataManager : IScoreboardDataManager
    {
        public PlayerDataList LoadData(string jsonFileName)
        {
            string path = Application.dataPath + $"/{jsonFileName}.json";

            if (System.IO.File.Exists(path))
            {
                string jsonData = System.IO.File.ReadAllText(path);
                return JsonUtility.FromJson<PlayerDataList>(jsonData);
            }
            else
            {
                // Create an empty PlayerDataList
                PlayerDataList newData = new PlayerDataList();
                // Serialize the empty PlayerDataList to JSON
                string newJsonData = JsonUtility.ToJson(newData);
                // Write the JSON data to a new file
                System.IO.File.WriteAllText(path, newJsonData);
                // Return the empty PlayerDataList
                return newData;
            }
        }

        public void SaveData(PlayerDataList data, string jsonFileName)
        {
            // Load existing player data
            PlayerDataList existingData = LoadData(jsonFileName);
            
            // Iterate through the provided player data
            foreach (PlayerData newData in data.playerDataList)
            {
                // Check if the player already exists in the existing data
                PlayerData existingPlayer = existingData.playerDataList.FirstOrDefault(p => p.name == newData.name && p.team == newData.team);

                if (existingPlayer != null)
                {
                    // Update the score of the existing player
                    existingPlayer.score = newData.score;
                }
                else
                {
                    // Add the new player entry if not found
                    existingData.playerDataList.Add(newData);
                }
            }
            // Save the updated data
            string jsonData = JsonUtility.ToJson(existingData);
            string path = Application.dataPath + $"/{jsonFileName}.json";
            System.IO.File.WriteAllText(path, jsonData);
        }
        
        private PlayerDataList CreateNewJsonFile(string jsonFileName)
        {
            PlayerDataList dataWrapper = new PlayerDataList();
            return dataWrapper;
        }
    }
}