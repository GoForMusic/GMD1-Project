using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
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
        
        private int _currentLevel = 1;
        private float _currentExperience = 0f;
        private float _maxExperience = 100f; // Initial max experience
        private float _levelUpMultiplier;
        private int _maxLevel;
        
        public void Init(float levelUpMultiplier, int maxLevel)
        {
            _levelUpMultiplier = levelUpMultiplier;
            _maxLevel = maxLevel;
            levelText.text = _currentLevel+"";
            UpdateExperience(0);
        }
        
        private void Update()
        {
            // Update flags count based on the number of game objects with the same tag
            int flagsCount = GameObject.FindGameObjectsWithTag(gameObject.tag).Length-1;
            UpdateFlagsText(flagsCount);
        }

        public void UpdateHealth(float currentHealth, float maxHealth)
        {
            float normalizedHealth = currentHealth / maxHealth;
            healthSlider.value = normalizedHealth;
            healthText.text = $"{currentHealth}/{maxHealth}";
        }
        
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
        
        public void ShowLevelUpImage(bool show)
        {
            levelUpImage.gameObject.SetActive(show);
        }
        
        public void UpdateDamageText(float damageAmount)
        {
            damageText.text = $"+ {damageAmount} DMG";
        }

        public void UpdateFlagsText(int flagsCount)
        {
            flagsText.text = "x " + flagsCount;
        }

        public int GetCurrentLevel()
        {
            return _currentLevel;
        }

    }
}