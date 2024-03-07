using System.Collections;
using System.Collections.Generic;
using Control;
using UnityEngine;

public class SpawnMinions : MonoBehaviour
{
    [Header("Minion Que for team")]
    [SerializeField]
    private List<GameObject> _minions;
    
    [Header("Spawner Properties")]
    [SerializeField] private float _spawnInterval = 30.0f;
    [SerializeField] private float _delayBetweenMinions=1f;

    [SerializeField]
    private PatrolPath _patrolPath;
    
    private void Start()
    {
        _patrolPath = GetComponent<PatrolPath>();
        if (_patrolPath == null)
        {
            Debug.LogError("PatrolPath component not found on the spawner object!");
        }
        else
        {
            StartCoroutine(SpawnMinionsAsync());
        }
    }

    private IEnumerator SpawnMinionsAsync()
    {
        while (true)
        {
            for (int i = 0; i < _minions.Count; i++)
            {
                SpawnRegularMinions(_minions[i]);
                yield return new WaitForSeconds(_delayBetweenMinions);
            }

            yield return new WaitForSeconds(_spawnInterval - _delayBetweenMinions);
        }
    }

    private void SpawnRegularMinions(GameObject minion)
    {
        GameObject objectSpawned = Instantiate(minion, transform.position, transform.rotation);
        objectSpawned.GetComponentInChildren<MinionAI>().patrolPath = _patrolPath;
    }
}
