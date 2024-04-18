using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Gameplay
{
    public class Flag : MonoBehaviour
    {
        [Header("Flag properties")]
        [SerializeField]
        private float captureTime = 2f;
        
        public Material[] teamMaterial;
        public Material[] minimapMaterialMark;
        
        private bool _capturing = false;
        private IEnumerator _captureCoroutine;
        private string _currentCapturingTeam = "";
        
        private Renderer _flagRenderer;
        private Renderer _flagMiniMapRenderer;

        private void Start()
        {
            _flagRenderer = GetComponentsInChildren<Renderer>()[1];
            _flagMiniMapRenderer = GetComponentsInChildren<Renderer>()[0];
        }

        private void OnTriggerEnter(Collider other)
        {
            // Check if the entering GameObject is a player or minion
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                if (!gameObject.CompareTag(other.tag+"Flag"))
                {
                    // Check if the entering GameObject belongs to one of the allowed teams
                    string team = other.tag;
                    if (!_capturing || _currentCapturingTeam!=team)
                    {
                        // Start capturing
                        _currentCapturingTeam = team;
                        _captureCoroutine = CaptureCoroutine();
                        StartCoroutine(_captureCoroutine);
                        Debug.Log("Capture started by team: " + _currentCapturingTeam);
                    }
                    else
                    {
                        // Check if the capturing team matches the entering GameObject's team
                        if (_currentCapturingTeam != team)
                        {
                            // Reset capture progress if the entering team is different
                            StopCoroutine(_captureCoroutine);
                            _capturing = false;
                            _currentCapturingTeam = "";
                            Debug.Log("Capture interrupted. Different team entered.");
                        }
                    }
                }
            }
        }
        
        private void OnTriggerExit(Collider other)
        {
            // If a player or minion exits the collider, stop capturing
            if (_capturing && (other.CompareTag("Team1") || other.CompareTag("Team2")))
            {
                // Check if the exiting GameObject is from the same team as the current capturing team
                if (!other.CompareTag(_currentCapturingTeam))
                {
                    StopCoroutine(_captureCoroutine);
                    _capturing = false;
                    _currentCapturingTeam = "";
                    Debug.Log("Capture interrupted. GameObject exited collider.");
                }
            }
        }
        
        
        
        private void OnTriggerStay(Collider other)
        {
            // Check if an enemy team is present within the capture zone while the flag is being captured
            if (_capturing && (other.CompareTag("Team1") || other.CompareTag("Team2")) && !other.CompareTag(_currentCapturingTeam))
            {
                // Check if any enemy team members are present within the capture zone
                Collider[] colliders = Physics.OverlapSphere(transform.position, transform.localScale.magnitude / 2);
                foreach (var collider in colliders)
                {
                    Debug.Log("Collider in range: " + collider.tag);
                }
                bool enemyPresent = colliders.Any(collider => collider.CompareTag("Team1") || collider.CompareTag("Team2"));

                
                // If no enemy team members are present, start the recapture process
                if (!enemyPresent)
                {
                    StartCoroutine(_captureCoroutine);
                }
                else
                {
                    // Interrupt capturing if an enemy team remains within the capture zone
                    StopCoroutine(_captureCoroutine);
                    _capturing = false;
                    _currentCapturingTeam = "";
                    Debug.Log("Capture interrupted. Enemy team present.");
                }
            }
        }
        
        private IEnumerator CaptureCoroutine()
        {
            _capturing = true;
            float timer = 0f;
            while (timer < captureTime)
            {
                yield return null;
                timer += Time.deltaTime;
                float progress = timer / captureTime;
                //Debug.Log("Capture progress: " + (progress * 100).ToString("F2") + "%");
            }

            // Capture completed
            Debug.Log("Flag captured by team: " + _currentCapturingTeam);
            // Implement capture completion actions here (e.g., change flag color, award points)
            // Change flag material based on the capturing team
            if (_currentCapturingTeam == "Team1")
            {
                _flagRenderer.material = teamMaterial[0];
                _flagMiniMapRenderer.material = minimapMaterialMark[0];
            }
            else if (_currentCapturingTeam == "Team2")
            {
                _flagRenderer.material = teamMaterial[1];
                _flagMiniMapRenderer.material = minimapMaterialMark[1];
            }
            gameObject.tag = _currentCapturingTeam+"Flag";
            
            _capturing = false;
            _currentCapturingTeam = "";
        }
    }
}