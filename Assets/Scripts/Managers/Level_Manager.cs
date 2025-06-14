using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class Level_Manager : MonoBehaviour
{
    private static Level_Manager instance;
    public static Level_Manager Instance => instance;
    
    [SerializeField] private List<PoolType> whatPoolTypes = new List<PoolType>();
    [SerializeField] private Transform spawnTransform;
    private Difficulty lastDifficulty;
    
    private List<GameObject> activeEnemies = new List<GameObject>();
    
    [Tooltip("0 = Easy, 1 = Medium, 2 = Hard")]
    [SerializeField, Range(1,10)] List<int> amountOfEnemies;
    [SerializeField] float spawnRadius;
    
    public static UnityAction OnGameOver;
    public static UnityAction OnGameWin;
    
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }
    
    public void StartLevel(Difficulty difficulty)
    {
        Time.timeScale = 1;
        lastDifficulty = difficulty;
        Player_Manager.Instance.MovementHandler.ResetPlayer();
        
        for (int i = 0; i < amountOfEnemies[(int)difficulty]; i++)
        {
            var enemy = Pool_Manager.Instance.GetObjectFromPool(PoolType.Enemy);
            Vector2 randomPoint = Random.insideUnitCircle;
            enemy.transform.position = new Vector3(randomPoint.x, 0, randomPoint.y) * spawnRadius;
            activeEnemies.Add(enemy);
        }
    }

    public void StartLevel()
    {
        StartLevel(lastDifficulty);
    }

    private void OnActiveEnemyDied(GameObject obj)
    {
        for(int i = 0; i < activeEnemies.Count; i++)
        {
            if (activeEnemies[i] == obj.gameObject)
            {
                activeEnemies.RemoveAt(i);
                Pool_Manager.Instance.ReturnToPool(obj.gameObject, PoolType.Enemy );
                break;
            }
        }
        if (activeEnemies.Count == 0)
        {
            //Level Complete
            OnGameWin?.Invoke();
            Debug.Log("Level Complete");
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("OnCollisionEnter");
        if (other.gameObject.CompareTag("Enemy"))
        {
            OnActiveEnemyDied(other.gameObject);
            
            return;
        }
        if (other.gameObject.CompareTag("Player"))
        {
            //Game Over
            OnGameOver?.Invoke();
            Debug.Log("Game Over");
        }
        
    }

    public void ResetLevel()
    {
        foreach (var enemy in activeEnemies)
        {
            Pool_Manager.Instance.ReturnToPool(enemy, PoolType.Enemy);
        }
        activeEnemies.Clear();
        Player_Manager.Instance.MovementHandler.ResetPlayer();
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
    }
}

public enum Difficulty
{
    Normal = 0,
    Infinite = 1,
}
