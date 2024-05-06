using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace UI
{
    public class UIManager : MonoBehaviour
    {
        public Slider healthSlider;
        public TextMeshProUGUI healthText;
        public Slider experienceSlider;
        public TextMeshProUGUI experienceText;
        public TextMeshProUGUI levelText;
        public GameObject levelUpImage;
        public TextMeshProUGUI damageText;
        public TextMeshProUGUI flagsText;
        public TextMeshProUGUI playerScore;
        private TimerController _timerController;
        
        //Show the Go to MainMenu
        public GameObject mainMenuElement;
        public EventSystem uiEventSystem;
        
        private int _currentLevel = 1;
        private float _currentExperience = 0f;
        private float _maxExperience = 100f; // Initial max experience
        private float _levelUpMultiplier;
        private int _maxLevel;
        private int _score = 0;
        private int _flagCount;
        
        /// <summary>
        /// Initializes the UI manager with level up multiplier and max level.
        /// </summary>
        /// <param name="levelUpMultiplier">The multiplier for leveling up.</param>
        /// <param name="maxLevel">The maximum level.</param>
        public void Init(float levelUpMultiplier, int maxLevel)
        {
            // Find the TimerController component
            _timerController = FindObjectOfType<TimerController>();
            _timerController.TenSecondsEvent += OnTenSecondsEvent;
            
            _levelUpMultiplier = levelUpMultiplier;
            _maxLevel = maxLevel;
            levelText.text = _currentLevel+"";
            UpdateExperience(0);
        }

        private void LateUpdate()
        {
            // Update flags count based on the number of game objects with the same tag
            _flagCount = GameObject.FindGameObjectsWithTag(gameObject.tag).Length-1;
            UpdateFlagsText(_flagCount);
        }

        /// <summary>
        /// Updates the health UI with current and max health values.
        /// </summary>
        /// <param name="currentHealth">The current health value.</param>
        /// <param name="maxHealth">The maximum health value.</param>
        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float normalizedHealth = currentHealth / maxHealth;
            healthSlider.value = normalizedHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
        
        /// <summary>
        /// Updates the experience UI with the gained experience.
        /// </summary>
        /// <param name="gainExperience">The amount of experience gained.</param>
        public void UpdateExperience(float gainExperience)
        {
            float totalExperience = _currentExperience + gainExperience;

            // Check if level up is required
            if (totalExperience >= _maxExperience && _currentLevel <= _maxLevel)
            {
                float remainingExperience = totalExperience - _maxExperience;
                LevelUp();
                //Apply the remaining experience
                UpdateExperience(remainingExperience);
            }
            else
            {
                _currentExperience = totalExperience;
                experienceSlider.value = _currentExperience / _maxExperience;
                experienceText.text = $"{_currentExperience}/{_maxExperience}";
            }
            UpdateScore(Mathf.RoundToInt(gainExperience));
        }
        
        private void LevelUp()
        {
            _currentLevel++;
            levelText.text = _currentLevel+"";
            ShowLevelUpImage(true);

            // Increase max experience for next level
            _maxExperience *= _levelUpMultiplier;
            _currentExperience = 0f;
            experienceSlider.value = _currentExperience / _maxExperience;
            experienceText.text = $"{_currentExperience}/{_maxExperience}";
            
            // Start coroutine to hide level up image after delay
            StartCoroutine(HideLevelUpImageAfterDelay());
        }
        
        private IEnumerator HideLevelUpImageAfterDelay()
        {
            yield return new WaitForSeconds(5f);
            ShowLevelUpImage(false);
        }
        
        /// <summary>
        /// Shows or hides the level up image.
        /// </summary>
        /// <param name="show">Boolean value indicating whether to show or hide the image.</param>
        public void ShowLevelUpImage(bool show)
        {
            levelUpImage.gameObject.SetActive(show);
        }
        
        /// <summary>
        /// Updates the damage text UI with the specified damage amount.
        /// </summary>
        /// <param name="damageAmount">The amount of damage.</param>
        public void UpdateDamageText(float damageAmount)
        {
            damageText.text = $"+ {damageAmount} DMG";
        }

        /// <summary>
        /// Updates the flags text UI with the specified number of flags.
        /// </summary>
        /// <param name="flagsCount">The number of flags.</param>
        public void UpdateFlagsText(int flagsCount)
        {
            flagsText.text = "x " + flagsCount;
        }
        
        private void OnTenSecondsEvent()
        {
            _score += 10 * (_flagCount==0?1:_flagCount);
            UpdateScoreUI(); // Pass 0 or any other default value as needed
        }
        
        private void UpdateScore(int scoreIncrease)
        {
            _score += scoreIncrease;
            UpdateScoreUI();
        }

        private void UpdateScoreUI()
        {
            playerScore.text= "Score: " + _score;
        }
        
        /// <summary>
        /// Gets the current score.
        /// </summary>
        /// <returns>The current score.</returns>
        public int GetCurrentScore()
        {
            return _score;
        }

        /// <summary>
        /// Shows the main menu UI element.
        /// </summary>
        public void ShowMenuUI()
        {
            mainMenuElement.SetActive(true);
            uiEventSystem.SetSelectedGameObject(mainMenuElement.GetComponentInChildren<Button>().gameObject);
        }
        
    }
}