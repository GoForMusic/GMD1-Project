---
title: "Blog 5 Developer post 3"
author: "Adrian-Cristian Militaru"
date: "11-05-2024"
version: "0.1"
---

# Game Development Progress Report 3

## Introduction

Welcome back to another update on the development of our Unity game project! In this blog post, I'll cover the implementation of saving game progress to a file, displaying the scoreboard, and applying final touches to enhance the overall gameplay experience.

## Saving Game Progress

### Save System Implementation

To enable players to save their scores locally, I've implemented a robust save system. This system records the player's name, team, and total score when the game is over and saves it to a JSON file.

```csharp
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
    }
```

### File Management

For file management, I create a method that will generate a file for each level. For example, "level1" will have a "level1.json" file to store player data for that match.

The json format will look like:

```json
{
  "playerDataList": [
    {
      "name": "ABC",
      "score": 3430,
      "team": "team1"
    },
    {
      "name": "BCB",
      "score": 1390,
      "team": "team2"
    }
  ]
}
```

## Displaying the Scoreboard

Players can view the scoreboard from the main menu when selecting a specific level. For example, there's a button called "ForestAttack", players can see the scoreboard for that level. If they choose to play, they will click on the button and input their names.

![UI Scoreboard](/blog/resources/blog05/ScoreboardUI.png)

To transport player input names to another scene, I've used two static variables that will later be stored in the JSON file along with the score.

```csharp
namespace Model
{
    /// <summary>
    /// Provides static access to player names, allowing them to be set and retrieved throughout the game scenes.
    /// </summary>
    public class InputPlayersName : MonoBehaviour
    {
        /// <summary>
        /// Gets or sets the name of Player 1.
        /// </summary>
        public static string Player1Name { get; set; }
        /// <summary>
        /// Gets or sets the name of Player 2.
        /// </summary>
        public static string Player2Name { get; set; }
    }
}
```

## Final Touches

A final touch is the redesign of the input system to accommodate gameplay on an arcade machine using two different controllers, each for a player. For PC, players will use the keyboard for player 1 (WASD for navigation) and (E for attack) and the controller for player 2.

### Kow bugs

One known bug remain to flag capture mechanics. There's an issue determining who captures the flag and in what context. This is particularly challenging when the enemy flag has enemy minions. If a player attempts to capture the flag by attacking and killing the minions, the capturing progress begins. However, if the player leaves the area and returns, the process does not resume. There are likely issues with the OnTrigger and OnTriggerExit events.

## Conclusion

In this blog post, I've create the same scoreboard function, displaying the scoreboard on UI, and tackling final touches, including known bugs.

Despite the progress made, I've encountered a known bug related to flag capture mechanics. This issue arises from complexities in triggering and exiting capture zones, particularly in contested scenarios involving enemy minions. Addressing this bug will require careful debugging and refinement of trigger event logic.(But being busy with my bachelor, I will pass this bugg as is not breaking the game)