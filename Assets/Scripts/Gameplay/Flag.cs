using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Control;
using Interfaces.Core;
using UnityEngine;

namespace Gameplay
{
    public class Flag : MonoBehaviour
    {
        [Header("Flag properties")]
        [SerializeField]
        private float captureTime = 2f;
        [SerializeField]
        [Range(1,2)]
        private int _teamID; // Team ID (1 for Team1, 2 for Team2)
        
        public Material[] teamMaterial;
        public Material[] minimapMaterialMark;
        
        private bool _capturing = false;
        private IEnumerator _captureCoroutine;
        private string _currentCapturingTeam;
        
        private Renderer _flagRenderer;
        private Renderer _flagMiniMapRenderer;
        
        [SerializeField]
        private List<GameObject> _minionsInCaptureZone;
        
        private void Start()
        {
            _flagRenderer = GetComponentsInChildren<Renderer>()[1];
            _flagMiniMapRenderer = GetComponentsInChildren<Renderer>()[0];
            _minionsInCaptureZone = new List<GameObject>();
            SetupFlag();
        }
        
        private void SetupFlag()
        {
            // Convert team ID to team name
            string teamName = _teamID == 1 ? "Team1Flag" : "Team2Flag";

            // Change flag material based on the specified team
            switch (teamName)
            {
                case "Team1Flag":
                    _flagRenderer.material = teamMaterial[0];
                    _flagMiniMapRenderer.material = minimapMaterialMark[0];
                    _currentCapturingTeam = "Team2";
                    break;
                case "Team2Flag":
                    _flagRenderer.material = teamMaterial[1];
                    _flagMiniMapRenderer.material = minimapMaterialMark[1];
                    _currentCapturingTeam = "Team1";
                    break;
            }
            
            // Set the tag based on the specified team
            gameObject.tag = teamName;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            // Check if the entering GameObject is a player or minion
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                other.gameObject.GetComponent<IHealthProvider>().GetHealth().OnDeathHandle += DeathHandler;
                _minionsInCaptureZone.Add(other.gameObject);
                // Check if the entering GameObject belongs to one of the allowed teams
                if (!_capturing && AreAllMembersSameTeam())
                {
                    if (!gameObject.CompareTag(other.tag + "Flag"))
                    {
                        _currentCapturingTeam = other.tag;
                        _captureCoroutine = CaptureCoroutine();
                        StartCoroutine(_captureCoroutine);
                    }
                }
            }
        }

        private bool AreAllMembersSameTeam()
        {
            foreach (var minion in _minionsInCaptureZone)
            {
                if (!minion.CompareTag(_currentCapturingTeam))
                {
                    return false;
                }
            }

            return true;
        }
        
        private void OnTriggerExit(Collider other)
        {
            // If a player or minion exits the collider, stop capturing
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                other.gameObject.GetComponent<IHealthProvider>().GetHealth().OnDeathHandle -= DeathHandler;
                _minionsInCaptureZone.Remove(other.gameObject);
                // Check if the exiting GameObject is from the same team as the current capturing team
                if (_capturing && AreAllMembersSameTeam())
                {
                    StopCoroutine(_captureCoroutine);
                    _capturing = false;
                    _currentCapturingTeam = gameObject.tag; // Reset current capturing team
                }
            }
        }
        
        
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                if (AreAllMembersSameTeam() && !gameObject.CompareTag(other.tag + "Flag"))
                {
                    Debug.Log("das?");
                    _captureCoroutine = CaptureCoroutine();
                    StartCoroutine(_captureCoroutine);
                }
               
            }
        }

        private void DeathHandler(IHealth obj)
        {
            GameObject gameobject = _minionsInCaptureZone.Find(g =>
                g.gameObject.GetComponent<IHealthProvider>().GetHealth() == obj);
            if (gameobject != null && _minionsInCaptureZone.Contains(gameobject))
            {
                _minionsInCaptureZone.Remove(gameobject);
                // Additional logic if needed
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
            // Change flag material based on the capturing team
            if (_currentCapturingTeam == "Team1")
            {
                gameObject.tag = "Team1Flag";
                _flagRenderer.material = teamMaterial[0];
                _flagMiniMapRenderer.material = minimapMaterialMark[0];
            }
            else if (_currentCapturingTeam == "Team2")
            {
                gameObject.tag = "Team2Flag";
                _flagRenderer.material = teamMaterial[1];
                _flagMiniMapRenderer.material = minimapMaterialMark[1];
            }
            
            _capturing = false;
        }
    }
}