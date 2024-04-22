using System.Collections;
using System.Collections.Generic;
using Control;
using Core;
using PoolManager;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    [Header("Spawn Configuration")]
    [SerializeField]
    private List<string> _spawnConfigurations;
    
    [Header("Spawner Properties")]
    [SerializeField] private float _spawnInterval = 30.0f;
    [SerializeField] private float _delayBetweenMinions=1f;

    [SerializeField]
    private PatrolPath _patrolPath;
    
    // Reference to the minion pool manager
    private MinionPoolManager _minionPoolManager;
    
    private void Start()
    {
        _patrolPath = GetComponent<PatrolPath>();
        if (_patrolPath == null)
        {
            Debug.LogError("PatrolPath component not found on the spawner object!");
        }
        else
        {
            _minionPoolManager = FindObjectOfType<MinionPoolManager>();
            
            StartCoroutine(SpawnMinionsAsync());
        }
    }

    private IEnumerator SpawnMinionsAsync()
    {
        while (true)
        {
            foreach (var key in _spawnConfigurations)
            {
                Vector3 spawnLocation = _patrolPath.GetWaypoints()[0];
                GameObject minion = _minionPoolManager.GetMinionFromPool(key,spawnLocation, transform.rotation);

                // Check if the pool size needs to be increased
                if (minion == null)
                {
                    _minionPoolManager.IncreasePoolSize(key);
                    minion = _minionPoolManager.GetMinionFromPool(key, spawnLocation, transform.rotation);
                }
                
                if (minion != null)
                {
                    SpawnRegularMinions(minion);
                }
                
                yield return new WaitForSeconds(_delayBetweenMinions);
            }
            
            yield return new WaitForSeconds(_spawnInterval - _delayBetweenMinions);
        }
    }

    private void SpawnRegularMinions(GameObject minion)
    {
        minion.SetActive(true);
        minion.GetComponentInChildren<MinionAI>().patrolPath = _patrolPath;
    }
}
