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
        [SerializeField]
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
        
        /// <summary>
        /// Sets up the initial configuration of the flag based on the team ID.
        /// </summary>
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
        
        /// <summary>
        /// Adds a GameObject to the capture zone and starts capture if conditions are met.
        /// </summary>
        /// <param name="other">The collider that entered the capture zone.</param>
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
        
        /// <summary>
        /// Removes a GameObject from the capture zone and stops capture if conditions are met.
        /// </summary>
        /// <param name="other">The collider that exited the capture zone.</param>
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
        
        /// <summary>
        /// Handles the continuous presence of a GameObject within the capture zone and initiates capturing if conditions are met.
        /// </summary>
        /// <param name="other">The collider that stayed within the capture zone.</param>
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                if (AreAllMembersSameTeam() && !gameObject.CompareTag(other.tag + "Flag"))
                {
                    if (!_capturing)
                    {
                        _captureCoroutine = CaptureCoroutine();
                        StartCoroutine(_captureCoroutine);
                    }
                }
               
            }
        }

        /// <summary>
        /// Manages the death event of objects within the capture zone and updates the capturing team's status.
        /// </summary>
        /// <param name="obj">The health component of the deceased object.</param>
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
        
        /// <summary>
        /// Executes the capturing process over a set duration and updates the flag status upon completion.
        /// </summary>
        private IEnumerator CaptureCoroutine()
        {
            _capturing = true;
            Debug.Log("Start capturing!");
            yield return new WaitForSeconds(captureTime);
            Debug.Log("Capture completed!");
            // Capture completed
            // Change flag material based on the capturing team
            if (_currentCapturingTeam == "Team1")
            {
                gameObject.tag = "Team1Flag";
                _flagRenderer.material = teamMaterial[0];
                _flagMiniMapRenderer.material = minimapMaterialMark[0];
                _currentCapturingTeam = "Team2";
            }
            else if (_currentCapturingTeam == "Team2")
            {
                gameObject.tag = "Team2Flag";
                _flagRenderer.material = teamMaterial[1];
                _flagMiniMapRenderer.material = minimapMaterialMark[1];
                _currentCapturingTeam = "Team1";
            }
            
            
            _capturing = false;
        }
    }
}