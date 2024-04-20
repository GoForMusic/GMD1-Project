using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        private List<string> _tagsInCaptureZone;
        
        private void Start()
        {
            _flagRenderer = GetComponentsInChildren<Renderer>()[1];
            _flagMiniMapRenderer = GetComponentsInChildren<Renderer>()[0];
            _tagsInCaptureZone = new List<string>();
            _currentCapturingTeam = "";
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
                    _currentCapturingTeam = "Team1Flag";
                    break;
                case "Team2Flag":
                    _flagRenderer.material = teamMaterial[1];
                    _flagMiniMapRenderer.material = minimapMaterialMark[1];
                    _currentCapturingTeam = "Team2Flag";
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
                _tagsInCaptureZone.Add(other.tag);
                // Check if the entering GameObject belongs to one of the allowed teams
                if (!_capturing && AreAllMembersSameTeam())
                {
                    if (!gameObject.CompareTag(other.tag + "Flag"))
                    {
                        _currentCapturingTeam = other.tag;
                        _captureCoroutine = CaptureCoroutine();
                        StartCoroutine(_captureCoroutine);
                        Debug.Log("Capture started by team: " + _currentCapturingTeam);
                    }
                }
            }
        }

        private bool AreAllMembersSameTeam()
        {
            return _tagsInCaptureZone.All(tag => tag == "Team1") || _tagsInCaptureZone.All(tag => tag == "Team2");
        }
        
        private void OnTriggerExit(Collider other)
        {
            // If a player or minion exits the collider, stop capturing
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                _tagsInCaptureZone.Remove(other.tag);
                // Check if the exiting GameObject is from the same team as the current capturing team
                if (_capturing && !AreAllMembersSameTeam())
                {
                    StopCoroutine(_captureCoroutine);
                    _capturing = false;
                    _currentCapturingTeam = ""; // Reset current capturing team
                }
            }
        }
        
        //TODO: Needs to fix when the emeny die in his range
        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Team1") || other.CompareTag("Team2"))
            {
                if (!_tagsInCaptureZone.Contains(other.tag))
                {
                    if (!_capturing && AreAllMembersSameTeam())
                    {
                        _captureCoroutine = CaptureCoroutine();
                        StartCoroutine(_captureCoroutine);
                    }
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