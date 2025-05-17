using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class Level_Manager : MonoBehaviour
{
    private static Level_Manager instance;
    public static Level_Manager Instance => instance;
    
    [SerializeField] private List<PoolType> whatPoolTypes;
    [SerializeField] private Transform spawTransform;
    
    private List<GameObject> activeEnemies;
    
    [SerializeField] int amountOfEnemies;
    [SerializeField] float spawnRadius;
    
    void Start()
    {
        SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < amountOfEnemies; i++)
        {
            var enemy = Pool_Manager.Instance.GetObjectFromPool(PoolType.Enemy);
            activeEnemies.Add(enemy);
            Vector2 randomPoint = Random.insideUnitCircle;
            enemy.transform.position = new Vector3(randomPoint.x, 0, randomPoint.y) * spawnRadius;
        }
    }

    private void OnActiveEnemyDied(GameObject obj)
    {
        for(int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] == obj.gameObject)
            {
                activeEnemies.RemoveAt(i);
                Pool_Manager.Instance.ReturnToPool(obj.gameObject);
                break;
            }
        }
        if (activeEnemies.Count == 0)
        {
            //Level Complete
            Debug.Log("Level Complete");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            OnActiveEnemyDied(other.gameObject);
        }
        if (other.CompareTag("Player"))
        {
            //Game Over
            Debug.Log("Game Over");
        }
        
    }
}
