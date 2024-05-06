using Model;

namespace Interfaces.File
{
    /// <summary>
    /// Interface for managing scoreboard data operations, including loading and saving data.
    /// </summary>
    public interface IScoreboardDataManager
    {
        /// <summary>
        /// Loads player data from a JSON file.
        /// </summary>
        /// <param name="jsonFileName">The name of the JSON file from which to load the data.</param>
        /// <returns>A PlayerDataList object containing all loaded player data.</returns>
        PlayerDataList LoadData(string jsonFileName);
        /// <summary>
        /// Saves player data to a JSON file.
        /// </summary>
        /// <param name="data">The PlayerDataList object containing player data to save.</param>
        /// <param name="jsonFileName">The name of the JSON file to which the data will be saved.</param>
        void SaveData(PlayerDataList data, string jsonFileName);
    }
}