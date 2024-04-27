using Model;
using TMPro;
using UnityEngine;

namespace UI.Scoreboard
{
    public class ScoreboardEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _positionText;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _scoreText;
        
        
        public void Initialize(int position,PlayerData playerData)
        {            
            _positionText.text = position.ToString();
            _nameText.text = playerData.name;
            _scoreText.text = playerData.score.ToString();
        }
    }
}