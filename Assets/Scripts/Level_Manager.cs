using System.Collections.Generic;
using UnityEditor.SearchService;
using UnityEngine;

public class Level_Manager : MonoBehaviour
{
    private static Level_Manager instance;
    public static Level_Manager Instance => instance;
    
    [SerializeField] private List<PoolType> whatPoolTypes = new List<PoolType>();
    [SerializeField] private Transform spawnTransform;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    [Tooltip("0 = Easy, 1 = Medium, 2 = Hard")]
    [SerializeField, Range(1,10)] List<int> amountOfEnemies;
    [SerializeField] float spawnRadius;
    
    void Start()
    {
        StartLevel();
    }

    private void StartLevel(Difficulty difficulty = Difficulty.Easy)
    {
        for (int i = 0; i < amountOfEnemies[(int)difficulty]; i++)
        {
            var enemy = Pool_Manager.Instance.GetObjectFromPool(PoolType.Enemy);
            Vector2 randomPoint = Random.insideUnitCircle;
            enemy.transform.position = new Vector3(randomPoint.x, 0, randomPoint.y) * spawnRadius;
            activeEnemies.Add(enemy);
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

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}
