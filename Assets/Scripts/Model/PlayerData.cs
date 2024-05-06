using System;
using System.Collections.Generic;

namespace Model
{
    /// <summary>
    /// Represents a single player's data.
    /// </summary>
    [Serializable]
    public class PlayerData
    {
        /// <summary>
        /// The name of the player.
        /// </summary>
        public string name;
        /// <summary>
        /// The score achieved by the player.
        /// </summary>
        public int score;
        /// <summary>
        /// The team to which the player belongs.
        /// </summary>
        public string team;
    }

    /// <summary>
    /// A collection of player data used to store and manage multiple players' data.
    /// </summary>
    [Serializable]
    public class PlayerDataList
    {
        /// <summary>
        /// A list containing the data of all players.
        /// </summary>
        public List<PlayerData> playerDataList=new List<PlayerData>();
    }
}