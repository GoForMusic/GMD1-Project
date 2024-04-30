using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class TimerController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI timeLeftText;
    [SerializeField]
    private float totalTime = 600f;// 600- 10 minutes in seconds
    private float _timeLeft;

    // Event to be invoked every 10 seconds
    public event Action TenSecondsEvent;
    // Event to be invoked when the game is over
    public event Action GameOverEvent;
    
    private void Start()
    {
        _timeLeft = totalTime;
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (_timeLeft > 0)
        {
            _timeLeft -= Time.deltaTime;
            UpdateUI();

            // Check if it's time to invoke the event (every 10 seconds)
            if (_timeLeft % 10 < Time.deltaTime)
            {
                TenSecondsEvent?.Invoke();
            }
            
            yield return null;
        }

        // Invoke the game over event
        GameOverEvent?.Invoke();
        // You can add your game over logic here, like showing a game over screen or resetting the game
        Time.timeScale = 0;
    }

    private void UpdateUI()
    {
        int minutes = Mathf.FloorToInt(_timeLeft / 60f);
        int seconds = Mathf.FloorToInt(_timeLeft % 60f);
        timeLeftText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
