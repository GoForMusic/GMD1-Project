using Model;

namespace Interfaces.File
{
    public interface IScoreboardDataManager
    {
        PlayerDataList LoadData(string jsonFileName);
        void SaveData(PlayerDataList data, string jsonFileName);
    }
}