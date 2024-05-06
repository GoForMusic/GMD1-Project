using Model;
using TMPro;
using UnityEngine;

namespace UI.Scoreboard
{
    /// <summary>
    /// Represents a single entry in the scoreboard, displaying the position, player name, and score.
    /// </summary>
    public class ScoreboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _positionText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        /// <summary>
        /// Initializes the scoreboard entry with the provided position and player data.
        /// </summary>
        /// <param name="position">The position of the entry in the scoreboard.</param>
        /// <param name="playerData">The data of the player associated with this entry.</param>
        public void Initialize(int position,PlayerData playerData)
        {            
            _positionText.text = position.ToString();
            _nameText.text = playerData.name;
            _scoreText.text = playerData.score.ToString();
        }
    }
}