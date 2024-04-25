using System;
using System.Collections.Generic;

namespace Model
{
    [Serializable]
    public class PlayerData
    {
        public string name;
        public int score;
        public string team;
    }

    [Serializable]
    public class PlayerDataList
    {
        public List<PlayerData> playerDataList;
    }
}