using UnityEngine;

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